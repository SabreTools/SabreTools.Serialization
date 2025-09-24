using SabreTools.Serialization.ASN1;
using Xunit;

namespace SabreTools.Serialization.Test.ASN1
{
    // These tests are known to be incomplete due to the sheer number
    // of possible OIDs that exist. The tests below are a minimal
    // representation of functionality to guarantee proper behavior
    // not necessarily absolute outputs
    public class ObjectIdentifierTests
    {
        #region ASN.1

        [Fact]
        public void ASN1Notation_AlwaysNull()
        {
            ulong[]? values = null;
            string? actual = ObjectIdentifier.ParseOIDToASN1Notation(values); 
            Assert.Null(actual);
        }

        #endregion

        #region Dot Notation

        [Fact]
        public void DotNotation_NullValues_Null()
        {
            ulong[]? values = null;
            string? actual = ObjectIdentifier.ParseOIDToDotNotation(values); 
            Assert.Null(actual);
        }

        [Fact]
        public void DotNotation_EmptyValues_Null()
        {
            ulong[]? values = [];
            string? actual = ObjectIdentifier.ParseOIDToDotNotation(values); 
            Assert.Null(actual);
        }

        [Fact]
        public void DotNotation_Values_Formatted()
        {
            string expected = "0.1.2.3";
            ulong[]? values = [0, 1, 2, 3];
            string? actual = ObjectIdentifier.ParseOIDToDotNotation(values); 
            Assert.Equal(expected, actual);
        }

        #endregion

        #region Modified OID-IRI

        [Fact]
        public void ModifiedOIDIRI_NullValues_Null()
        {
            ulong[]? values = null;
            string? actual = ObjectIdentifier.ParseOIDToModifiedOIDIRI(values); 
            Assert.Null(actual);
        }

        [Fact]
        public void ModifiedOIDIRI_EmptyValues_Null()
        {
            ulong[]? values = [];
            string? actual = ObjectIdentifier.ParseOIDToModifiedOIDIRI(values); 
            Assert.Null(actual);
        }

        [Fact]
        public void ModifiedOIDIRI_Values_Formatted()
        {
            string expected = "/ITU-T/[question]/2/3";
            ulong[]? values = [0, 1, 2, 3];
            string? actual = ObjectIdentifier.ParseOIDToModifiedOIDIRI(values); 
            Assert.Equal(expected, actual);
        }

        #endregion

        #region OID-IRI

        [Fact]
        public void OIDIRI_NullValues_Null()
        {
            ulong[]? values = null;
            string? actual = ObjectIdentifier.ParseOIDToOIDIRINotation(values); 
            Assert.Null(actual);
        }

        [Fact]
        public void OIDIRI_EmptyValues_Null()
        {
            ulong[]? values = [];
            string? actual = ObjectIdentifier.ParseOIDToOIDIRINotation(values); 
            Assert.Null(actual);
        }

        [Fact]
        public void OIDIRI_Values_Formatted()
        {
            string expected = "/ITU-T/1/2/3";
            ulong[]? values = [0, 1, 2, 3];
            string? actual = ObjectIdentifier.ParseOIDToOIDIRINotation(values); 
            Assert.Equal(expected, actual);
        }

        #endregion
    }
}