using System;
using SabreTools.Data.Models.Nitro;
using SabreTools.IO.Extensions;

namespace SabreTools.Serialization.Wrappers
{
    public partial class Nitro
    {
        #region Encryption process variables

        private uint[] _cardHash = new uint[0x412];
        private uint[] _arg2 = new uint[3];

        #endregion

        #region Encrypt

        /// <summary>
        /// Encrypt secure area in the DS/DSi file
        /// </summary>s
        /// <param name="tableData">Blowfish table data as a byte array</param>
        /// <param name="force">Indicates if the operation should be forced</param>
        public void EncryptSecureArea(byte[] tableData, bool force)
        {
            // If we're forcing the operation, tell the user
            if (force)
            {
                Console.WriteLine("File is not verified due to force flag being set.");
            }
            // If we're not forcing the operation, check to see if we should be proceeding
            else
            {
                bool? isDecrypted = CheckIfDecrypted(out string? message);
                if (message != null)
                    Console.WriteLine(message);

                if (isDecrypted == null)
                {
                    Console.WriteLine("File has an empty secure area, cannot proceed");
                    return;
                }
                else if (!isDecrypted.Value)
                {
                    Console.WriteLine("File is already encrypted");
                    return;
                }
            }

            EncryptARM9(tableData);
            Console.WriteLine("File has been encrypted");
        }

        /// <summary>
        /// Encrypt the secure ARM9 region of the file, if possible
        /// </summary>
        /// <param name="tableData">Blowfish table data as a byte array</param>
        private void EncryptARM9(byte[] tableData)
        {
            // If the secure area is invalid, nothing can be done
            if (SecureArea == null)
                return;

            // Point to the beginning of the secure area
            int readOffset = 0;

            // Grab the first two blocks
            uint p0 = SecureArea.ReadUInt32LittleEndian(ref readOffset);
            uint p1 = SecureArea.ReadUInt32LittleEndian(ref readOffset);

            // Perform the initialization steps
            Init1(tableData);
            _arg2[1] <<= 1;
            _arg2[2] >>= 1;
            Init2();

            // Ensure alignment
            readOffset = 0x08;
            int writeOffset = 0x08;

            // Loop throgh the main encryption step
            uint size = 0x800 - 8;
            while (size > 0)
            {
                p0 = SecureArea.ReadUInt32LittleEndian(ref readOffset);
                p1 = SecureArea.ReadUInt32LittleEndian(ref readOffset);

                Encrypt(ref p1, ref p0);

                SecureArea.Write(ref writeOffset, p0);
                SecureArea.Write(ref writeOffset, p1);

                size -= 8;
            }

            // Replace the header explicitly
            readOffset = 0;
            writeOffset = 0;

            p0 = SecureArea.ReadUInt32LittleEndian(ref readOffset);
            p1 = SecureArea.ReadUInt32LittleEndian(ref readOffset);

            if (p0 == 0xE7FFDEFF && p1 == 0xE7FFDEFF)
            {
                p0 = Constants.MAGIC30;
                p1 = Constants.MAGIC34;
            }

            Encrypt(ref p1, ref p0);
            Init1(tableData);
            Encrypt(ref p1, ref p0);

            SecureArea.Write(ref writeOffset, p0);
            SecureArea.Write(ref writeOffset, p1);
        }

        /// <summary>
        /// Perform an encryption step
        /// </summary>
        /// <param name="arg1">First unsigned value to use in encryption</param>
        /// <param name="arg2">Second unsigned value to use in encryption</param>
        private void Encrypt(ref uint arg1, ref uint arg2)
        {
            uint a = arg1;
            uint b = arg2;
            for (int i = 0; i < 16; i++)
            {
                uint c = _cardHash[i] ^ a;
                a = b ^ Lookup(c);
                b = c;
            }

            arg2 = a ^ _cardHash[16];
            arg1 = b ^ _cardHash[17];
        }

        #endregion

        #region Decrypt

        /// <summary>
        /// Decrypt secure area in the DS/DSi file
        /// </summary>s
        /// <param name="tableData">Blowfish table data as a byte array</param>
        /// <param name="force">Indicates if the operation should be forced</param>
        public void DecryptSecureArea(byte[] tableData, bool force)
        {
            // If we're forcing the operation, tell the user
            if (force)
            {
                Console.WriteLine("File is not verified due to force flag being set.");
            }
            // If we're not forcing the operation, check to see if we should be proceeding
            else
            {
                bool? isDecrypted = CheckIfDecrypted(out string? message);
                if (message != null)
                    Console.WriteLine(message);

                if (isDecrypted == null)
                {
                    Console.WriteLine("File has an empty secure area, cannot proceed");
                    return;
                }
                else if (isDecrypted.Value)
                {
                    Console.WriteLine("File is already decrypted");
                    return;
                }
            }

            DecryptARM9(tableData);
            Console.WriteLine("File has been decrypted");
        }

        /// <summary>
        /// Decrypt the secure ARM9 region of the file, if possible
        /// </summary>
        /// <param name="tableData">Blowfish table data as a byte array</param>
        private void DecryptARM9(byte[] tableData)
        {
            // If the secure area is invalid, nothing can be done
            if (SecureArea == null)
                return;

            // Point to the beginning of the secure area
            int readOffset = 0;
            int writeOffset = 0;

            // Grab the first two blocks
            uint p0 = SecureArea.ReadUInt32LittleEndian(ref readOffset);
            uint p1 = SecureArea.ReadUInt32LittleEndian(ref readOffset);

            // Perform the initialization steps
            Init1(tableData);
            Decrypt(ref p1, ref p0);
            _arg2[1] <<= 1;
            _arg2[2] >>= 1;
            Init2();

            // Set the proper flags
            Decrypt(ref p1, ref p0);
            if (p0 == Constants.MAGIC30 && p1 == Constants.MAGIC34)
            {
                p0 = 0xE7FFDEFF;
                p1 = 0xE7FFDEFF;
            }

            SecureArea.Write(ref writeOffset, p0);
            SecureArea.Write(ref writeOffset, p1);

            // Ensure alignment
            readOffset = 0x08;
            writeOffset = 0x08;

            // Loop throgh the main encryption step
            uint size = 0x800 - 8;
            while (size > 0)
            {
                p0 = SecureArea.ReadUInt32LittleEndian(ref readOffset);
                p1 = SecureArea.ReadUInt32LittleEndian(ref readOffset);

                Decrypt(ref p1, ref p0);

                SecureArea.Write(ref writeOffset, p0);
                SecureArea.Write(ref writeOffset, p1);

                size -= 8;
            }
        }

        /// <summary>
        /// Perform a decryption step
        /// </summary>
        /// <param name="arg1">First unsigned value to use in decryption</param>
        /// <param name="arg2">Second unsigned value to use in decryption</param>
        private void Decrypt(ref uint arg1, ref uint arg2)
        {
            uint a = arg1;
            uint b = arg2;
            for (int i = 17; i > 1; i--)
            {
                uint c = _cardHash[i] ^ a;
                a = b ^ Lookup(c);
                b = c;
            }

            arg1 = b ^ _cardHash[0];
            arg2 = a ^ _cardHash[1];
        }

        #endregion

        #region Common

        /// <summary>
        /// Determine if the current file is already decrypted or not (or has an empty secure area)
        /// </summary>
        /// <param name="message">Optional message with more information on the result</param>
        /// <returns>True if the file has known values for a decrypted file, null if it's empty, false otherwise</returns>
        public bool? CheckIfDecrypted(out string? message)
        {
            // Return empty if the secure area is undefined
            if (SecureArea == null)
            {
                message = "Secure area is undefined. Cannot be encrypted or decrypted.";
                return null;
            }

            int offset = 0;
            uint firstValue = SecureArea.ReadUInt32LittleEndian(ref offset);
            uint secondValue = SecureArea.ReadUInt32LittleEndian(ref offset);

            // Empty secure area standard
            if (firstValue == 0x00000000 && secondValue == 0x00000000)
            {
                message = "Empty secure area found. Cannot be encrypted or decrypted.";
                return null;
            }

            // Improperly decrypted empty secure area (decrypt empty with woodsec)
            else if ((firstValue == 0xE386C397 && secondValue == 0x82775B7E)
                || (firstValue == 0xF98415B8 && secondValue == 0x698068FC)
                || (firstValue == 0xA71329EE && secondValue == 0x2A1D4C38)
                || (firstValue == 0xC44DCC48 && secondValue == 0x38B6F8CB)
                || (firstValue == 0x3A9323B5 && secondValue == 0xC0387241))
            {
                message = "Improperly decrypted empty secure area found. Should be encrypted to get proper value.";
                return true;
            }

            // Improperly encrypted empty secure area (encrypt empty with woodsec)
            else if ((firstValue == 0x4BCE88BE && secondValue == 0xD3662DD1)
                || (firstValue == 0x2543C534 && secondValue == 0xCC4BE38E))
            {
                message = "Improperly encrypted empty secure area found. Should be decrypted to get proper value.";
                return false;
            }

            // Properly decrypted nonstandard value (mastering issue)
            else if ((firstValue == 0xD0D48B67 && secondValue == 0x39392F23) // Dragon Quest 5 (EU)
                || (firstValue == 0x014A191A && secondValue == 0xA5C470B9)   // Dragon Quest 5 (USA)
                || (firstValue == 0x7829BC8D && secondValue == 0x9968EF44)   // Dragon Quest 5 (JP)
                || (firstValue == 0xC4A15AB8 && secondValue == 0xD2E667C8)   // Prince of Persia (EU)
                || (firstValue == 0xD5E97D20 && secondValue == 0x21B2A159))  // Prince of Persia (USA)
            {
                message = "Decrypted secure area for known, nonstandard value found.";
                return true;
            }

            // Properly decrypted prototype value
            else if (firstValue == 0xBA35F813 && secondValue == 0xB691AAE8)
            {
                message = "Decrypted secure area for prototype found.";
                return true;
            }

            // Strange, unlicenced values that can't determine decryption state
            else if ((firstValue == 0xE1D830D8 && secondValue == 0xE3530000) // Aquela Ball (World) (Unl) (Datel Games n' Music)
                || (firstValue == 0xDC002A02 && secondValue == 0x2900E612)   // Bahlz (World) (Unl) (Datel Games n' Music)
                || (firstValue == 0xE1A03BA3 && secondValue == 0xE2011CFF)   // Battle Ship (World) (Unl) (Datel Games n' Music)
                || (firstValue == 0xE3A01001 && secondValue == 0xE1A02001)   // Breakout!! DS (World) (Unl) (Datel Games n' Music)
                || (firstValue == 0xE793200C && secondValue == 0xE4812004)   // Bubble Fusion (World) (Unl) (Datel Games n' Music)
                || (firstValue == 0xE583C0DC && secondValue == 0x0A00000B)   // Carre Rouge (World) (Unl) (Datel Games n' Music)
                || (firstValue == 0x0202453C && secondValue == 0x02060164)   // ChainReaction (World) (Unl) (Datel Games n' Music)
                || (firstValue == 0xEBFFF218 && secondValue == 0xE31000FF)   // Collection (World) (Unl) (Datel Games n' Music)
                || (firstValue == 0x4A6CD003 && secondValue == 0x425B2301)   // DiggerDS (World) (Unl) (Datel Games n' Music)
                || (firstValue == 0xE3A00001 && secondValue == 0xEBFFFF8C)   // Double Skill (World) (Unl) (Datel Games n' Music)
                || (firstValue == 0x21043701 && secondValue == 0x45BA448C)   // DSChess (World) (Unl) (Datel Games n' Music)
                || (firstValue == 0xE59D0010 && secondValue == 0xE0833000)   // Hexa-Virus (World) (Unl) (Datel Games n' Music)
                || (firstValue == 0xE5C3A006 && secondValue == 0xE5C39007)   // Invasion (World) (Unl) (Datel Games n' Music)
                || (firstValue == 0xE1D920F4 && secondValue == 0xE06A3000)   // JoggleDS (World) (Unl) (Datel Games n' Music)
                || (firstValue == 0xE59F32EC && secondValue == 0xE5DD7011)   // London Underground (World) (Unl) (Datel Games n' Music)
                || (firstValue == 0xE08A3503 && secondValue == 0xE1D3C4B8)   // NumberMinds (World) (Unl) (Datel Games n' Music)
                || (firstValue == 0xE1A0C001 && secondValue == 0xE0031001)   // Paddle Battle (World) (Unl) (Datel Games n' Music)
                || (firstValue == 0xE1A03005 && secondValue == 0xE88D0180)   // Pop the Balls (World) (Unl) (Datel Games n' Music)
                || (firstValue == 0xE8BD4030 && secondValue == 0xE12FFF1E)   // Solitaire DS (World) (Unl) (Datel Games n' Music)
                || (firstValue == 0xE0A88006 && secondValue == 0xE1A00003)   // Squash DS (World) (Unl) (Datel Games n' Music)
                || (firstValue == 0xE51F3478 && secondValue == 0xEB004A02)   // Super Snake DS (World) (Unl) (Datel Games n' Music)
                || (firstValue == 0x1C200052 && secondValue == 0xFD12F013)   // Tales of Dagur (World) (Unl) (Datel Games n' Music)
                || (firstValue == 0x601F491E && secondValue == 0x041B880B)   // Tetris & Touch (World) (Unl) (Datel Games n' Music)
                || (firstValue == 0xE1A03843 && secondValue == 0xE0000293)   // Tic Tac Toe (World) (Unl) (Datel Games n' Music)
                || (firstValue == 0xE3530000 && secondValue == 0x13A03003)   // Warrior Training (World) (Unl) (Datel Games n' Music)
                || (firstValue == 0x02054A80 && secondValue == 0x02054B80))  // Zi (World) (Unl) (Datel Games n' Music)
            {
                message = "Unlicensed invalid value found. Unknown if encrypted or decrypted.";
                return null;
            }

            // Standard decryption values
            message = null;
            return firstValue == 0xE7FFDEFF && secondValue == 0xE7FFDEFF;
        }

        /// <summary>
        /// First common initialization step
        /// </summary>
        /// <param name="tableData">Blowfish table data as a byte array</param>
        private void Init1(byte[] tableData)
        {
            Buffer.BlockCopy(tableData, 0, _cardHash, 0, 4 * (1024 + 18));
            _arg2 = [GameCode, GameCode >> 1, GameCode << 1];
            Init2();
            Init2();
        }

        /// <summary>
        /// Second common initialization step
        /// </summary>
        private void Init2()
        {
            Encrypt(ref _arg2[2], ref _arg2[1]);
            Encrypt(ref _arg2[1], ref _arg2[0]);

            byte[] allBytes = [.. BitConverter.GetBytes(_arg2[0]),
                .. BitConverter.GetBytes(_arg2[1]),
                .. BitConverter.GetBytes(_arg2[2])];

            UpdateHashtable(allBytes);
        }

        /// <summary>
        /// Lookup the value from the hashtable
        /// </summary>
        /// <param name="v">Value to lookup in the hashtable</param>
        /// <returns>Processed value through the hashtable</returns>
        private uint Lookup(uint v)
        {
            uint a = (v >> 24) & 0xFF;
            uint b = (v >> 16) & 0xFF;
            uint c = (v >> 8) & 0xFF;
            uint d = (v >> 0) & 0xFF;

            a = _cardHash[a + 18 + 0];
            b = _cardHash[b + 18 + 256];
            c = _cardHash[c + 18 + 512];
            d = _cardHash[d + 18 + 768];

            return d + (c ^ (b + a));
        }

        /// <summary>
        /// Update the hashtable
        /// </summary>
        /// <param name="arg1">Value to update the hashtable with</param>
        private void UpdateHashtable(byte[] arg1)
        {
            for (int j = 0; j < 18; j++)
            {
                uint r3 = 0;
                for (int i = 0; i < 4; i++)
                {
                    r3 <<= 8;
                    r3 |= arg1[(j * 4 + i) & 7];
                }

                _cardHash[j] ^= r3;
            }

            uint tmp1 = 0;
            uint tmp2 = 0;
            for (int i = 0; i < 18; i += 2)
            {
                Encrypt(ref tmp1, ref tmp2);
                _cardHash[i + 0] = tmp1;
                _cardHash[i + 1] = tmp2;
            }
            for (int i = 0; i < 0x400; i += 2)
            {
                Encrypt(ref tmp1, ref tmp2);
                _cardHash[i + 18 + 0] = tmp1;
                _cardHash[i + 18 + 1] = tmp2;
            }
        }

        #endregion
    }
}
