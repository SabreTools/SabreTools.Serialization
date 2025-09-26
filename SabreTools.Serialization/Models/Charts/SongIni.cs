using System;

namespace SabreTools.Data.Models.Charts
{
    /// <see href="https://github.com/TheNathannator/GuitarGame_ChartFormats/tree/main/doc/FileFormats/song.ini"/> 
    /// <remarks>[song]/[Song]</remarks>
    public class SongIni
    {
        #region Song/Chart Metadata

        /// <summary>
        /// Title of the song.
        /// </summary>
        /// <remarks>name</remarks>
        public string? Name { get; set; }

        /// <summary>
        /// Artist(s) or band(s) behind the song.
        /// </summary>
        /// <remarks>artist</remarks>
        public string? Artist { get; set; }

        /// <summary>
        /// Title of the album the song is featured in.
        /// </summary>
        /// <remarks>album</remarks>
        public string? Album { get; set; }

        /// <summary>
        /// Genre of the song.
        /// </summary>
        /// <remarks>genre</remarks>
        public string? Genre { get; set; }

        /// <summary>
        /// Sub-genre for the song.
        /// </summary>
        /// <remarks>sub_genre</remarks>
        public string? SubGenre { get; set; }

        /// <summary>
        /// Year of the song’s release.
        /// </summary>
        /// <remarks>year</remarks>
        public string? Year { get; set; }

        /// <summary>
        /// Community member responsible for charting the song.
        /// </summary>
        /// <remarks>charter, frets</remarks>
        public string? Charter { get; set; }

        /// <summary>
        /// Version number for the song.
        /// </summary>
        /// <remarks>version</remarks>
        public long? Version { get; set; }

        /// <summary>
        /// Track number of the song within the album it's from.
        /// </summary>
        /// <remarks>album_track, track</remarks>
        public long? AlbumTrack { get; set; }

        /// <summary>
        /// Track number of the song within the playlist/setlist it's from.
        /// </summary>
        /// <remarks>playlist_track</remarks>
        public long? PlaylistTrack { get; set; }

        /// <summary>
        /// Length of the song's audio in milliseconds.
        /// </summary>
        /// <remarks>song_length</remarks>
        public long? SongLength { get; set; }

        /// <summary>
        /// Timestamp in milliseconds where the song preview starts.
        /// </summary>
        /// <remarks>preview_start_time</remarks>
        public long? PreviewStartTime { get; set; }

        /// <summary>
        /// Timestamp in milliseconds that the preview should stop at.
        /// </summary>
        /// <remarks>preview_end_time</remarks>
        public long? PreviewEndTime { get; set; }

        /// <summary>
        /// Flavor text for this song, usually shown after picking the song or during loading.
        /// </summary>
        /// <remarks>loading_phrase</remarks>
        public string? LoadingPhrase { get; set; }

        #endregion

        #region Song/Chart Metadata (Game-Specific)

        /// <summary>
        /// (FoFiX) Hex color to use in the song screen for the cassette.
        /// </summary>
        /// <remarks>cassettecolor</remarks>
        public string? CassetteColor { get; set; }

        /// <summary>
        /// (FoFiX) Miscellaneous tags for the chart.
        /// Only known valid value is `cover`.
        /// </summary>
        /// <remarks>tags</remarks>
        public string? Tags { get; set; }

        /// <summary>
        /// (PS) Two timestamps in milliseconds for preview start and end time.
        /// Example: `55000 85000`
        /// </summary>
        /// <remarks>preview</remarks>
        public long[]? Preview { get; set; }

        /// <summary>
        /// (CH) Playlist that the song should show up in.
        /// </summary>
        /// <remarks>playlist</remarks>
        public string? Playlist { get; set; }

        /// <summary>
        /// (CH) Sub-playlist that the song should show up in. 	
        /// </summary>
        /// <remarks>sub_playlist</remarks>
        public string? SubPlaylist { get; set; }

        /// <summary>
        /// (CH) Indicates if this song is a modchart.
        /// Meant for sorting purposes only.
        /// </summary>
        /// <remarks>modchart</remarks>
        public bool? Modchart { get; set; }

        /// <summary>
        /// (CH) Indicates if the song has lyrics or not.
        /// Meant for sorting purposes only.
        /// </summary>
        /// <remarks>lyrics</remarks>
        public bool? Lyrics { get; set; }

        #endregion

        #region Track Difficulties

        /// <summary>
        /// Overall difficulty of the song.
        /// </summary>
        /// <remarks>diff_band</remarks>
        public long? DiffBand { get; set; }

        /// <summary>
        /// Difficulty of the Lead Guitar track.
        /// </summary>
        /// <remarks>diff_guitar</remarks>
        public long? DiffGuitar { get; set; }

        /// <summary>
        /// Difficulty of the 6-Fret Lead track.
        /// </summary>
        /// <remarks>diff_guitarghl</remarks>
        public long? DiffGuitarGHL { get; set; }

        /// <summary>
        /// Difficulty of the Guitar Co-op track.
        /// </summary>
        /// <remarks>diff_guitar_coop</remarks>
        public long? DiffGuitarCoop { get; set; }

        /// <summary>
        /// Difficulty of the 6-Fret Guitar Co-op track.
        /// </summary>
        /// <remarks>diff_guitar_coop_ghl</remarks>
        public long? DiffGuitarCoopGHL { get; set; }

        /// <summary>
        /// Difficulty of the Pro Guitar track.
        /// </summary>
        /// <remarks>diff_guitar_real</remarks>
        public long? DiffGuitarReal { get; set; }

        /// <summary>
        /// Difficulty of the Pro Guitar 22-fret track.
        /// </summary>
        /// <remarks>diff_guitar_real_22</remarks>
        public long? DiffGuitarReal22 { get; set; }

        /// <summary>
        /// Difficulty of the Rhythm Guitar track.
        /// </summary>
        /// <remarks>diff_rhythm</remarks>
        public long? DiffRhythm { get; set; }

        /// <summary>
        /// Difficulty of the 6-Fret Rhythm Guitar track.
        /// </summary>
        /// <remarks>diff_rhythm_ghl</remarks>
        public long? DiffRhythmGHL { get; set; }

        /// <summary>
        /// Difficulty of the Bass Guitar track.
        /// </summary>
        /// <remarks>diff_bass</remarks>
        public long? DiffBass { get; set; }

        /// <summary>
        /// Difficulty of the 6-Fret Bass track.
        /// </summary>
        /// <remarks>diff_bassghl</remarks>
        public long? DiffBassGHL { get; set; }

        /// <summary>
        /// Difficulty of the Pro Bass track.
        /// </summary>
        /// <remarks>diff_bass_real</remarks>
        public long? DiffBassReal { get; set; }

        /// <summary>
        /// Difficulty of the Pro Bass 22-fret track.
        /// </summary>
        /// <remarks>diff_bass_real_22</remarks>
        public long? DiffBassReal22 { get; set; }

        /// <summary>
        /// Difficulty of the Drums track.
        /// </summary>
        /// <remarks>diff_drums</remarks>
        public long? DiffDrums { get; set; }

        /// <summary>
        /// Difficulty of the Pro Drums track.
        /// </summary>
        /// <remarks>diff_drums_real</remarks>
        public long? DiffDrumsReal { get; set; }

        /// <summary>
        /// Difficulty of the Drums Real track.
        /// </summary>
        /// <remarks>diff_drums_real_ps</remarks>
        public long? DiffDrumsRealPS { get; set; }

        /// <summary>
        /// Difficulty of the Keys track.
        /// </summary>
        /// <remarks>diff_keys</remarks>
        public long? DiffKeys { get; set; }

        /// <summary>
        /// Difficulty of the Pro Keys track.
        /// </summary>
        /// <remarks>diff_keys_real</remarks>
        public long? DiffKeysReal { get; set; }

        /// <summary>
        /// Difficulty of the Keys Real track.
        /// </summary>
        /// <remarks>diff_keys_real_ps</remarks>
        public long? DiffKeysRealPS { get; set; }

        /// <summary>
        /// Difficulty of the Vocals track.
        /// </summary>
        /// <remarks>diff_vocals</remarks>
        public long? DiffVocals { get; set; }

        /// <summary>
        /// Difficulty of the Harmonies track.
        /// </summary>
        /// <remarks>diff_vocals_harm</remarks>
        public long? DiffVocalsHarm { get; set; }

        /// <summary>
        /// Difficulty of the Dance track.
        /// </summary>
        /// <remarks>diff_dance</remarks>
        public long? DiffDance { get; set; }

        #endregion

        #region Chart Properties

        /// <summary>
        /// Forces the Drums track to be Pro Drums.
        /// </summary>
        /// <remarks>pro_drums, pro_drum (FoFiX)</remarks>
        public bool? ProDrums { get; set; }

        /// <summary>
        /// Forces the Drums track to be 5-lane.
        /// </summary>
        /// <remarks>five_lane_drums</remarks>
        public bool? FiveLaneDrums { get; set; }

        /// <summary>
        /// Specifies a voice type for the singer (either "male" or "female").
        /// </summary>
        /// <remarks>vocal_gender</remarks>
        public string? VocalGender { get; set; }

        /// <summary>
        /// Specifies a tuning for 17-fret Pro Guitar.
        /// </summary>
        /// <remarks>real_guitar_tuning</remarks>
        public string? RealGuitarTuning { get; set; }

        /// <summary>
        /// Specifies a tuning for 22-fret Pro Guitar.
        /// </summary>
        /// <remarks>real_guitar_22_tuning</remarks>
        public string? RealGuitar22Tuning { get; set; }

        /// <summary>
        /// Specifies a tuning for 17-fret Pro Bass.
        /// </summary>
        /// <remarks>real_bass_tuning</remarks>
        public string? RealBassTuning { get; set; }

        /// <summary>
        /// Specifies a tuning for 22-fret Pro Bass.
        /// </summary>
        /// <remarks>real_bass_22_tuning</remarks>
        public string? RealBass22Tuning { get; set; }

        /// <summary>
        /// Specifies the number of lanes for the right hand in Real Keys.
        /// </summary>
        /// <remarks>real_keys_lane_count_right</remarks>
        public long? RealKeysLaneCountRight { get; set; }

        /// <summary>
        /// Specifies the number of lanes for the left hand in Real Keys.
        /// </summary>
        /// <remarks>real_keys_lane_count_left</remarks>
        public long? RealKeysLaneCountLeft { get; set; }

        /// <summary>
        /// Delays the chart relative to the audio by the specified number of milliseconds.
        /// Higher = later notes. Can be negative.
        /// </summary>
        /// <remarks>delay</remarks>
        [Obsolete]
        public long? Delay { get; set; }

        /// <summary>
        /// Overrides the default sustain cutoff threshold with a specified value in ticks.
        /// </summary>
        /// <remarks>sustain_cutoff_threshold</remarks>
        [Obsolete]
        public long? SustainCutoffThreshold { get; set; }

        /// <summary>
        /// Overrides the default HOPO threshold with a specified value in ticks.
        /// </summary>
        /// <remarks>hopo_frequency</remarks>
        [Obsolete]
        public long? HopoFrequency { get; set; }

        /// <summary>
        /// Sets the HOPO threshold to be a 1/8th step.
        /// </summary>
        /// <remarks>eighthnote_hopo</remarks>
        [Obsolete]
        public bool? EighthNoteHopo { get; set; }

        /// <summary>
        /// Overrides the .mid note number for Star Power on 5-Fret Guitar.
        /// Valid values are 103 and 116.
        /// </summary>
        /// <remarks>multiplier_note, star_power_note (PS)</remarks>
        [Obsolete]
        public long? MultiplierNote { get; set; }

        #endregion

        #region Chart Properties (Game-Specific)

        /// <summary>
        /// (PS) Sets 5 to 4 Lane Drums Fallback Note
        /// </summary>
        /// <remarks>drum_fallback_blue</remarks>
        public bool? DrumFallbackBlue { get; set; }

        /// <summary>
        /// (FoFiX) Marks a song as a tutorial and hides it from Quickplay.
        /// </summary>
        /// <remarks>tutorial</remarks>
        public bool? Tutorial { get; set; }

        /// <summary>
        /// (FoFiX) Marks a song as a boss battle.
        /// </summary>
        /// <remarks>boss_battle</remarks>
        public bool? BossBattle { get; set; }

        /// <summary>
        /// (FoFiX) Overrides the natural HOPO threshold using numbers from 0 to 5.
        /// </summary>
        /// <remarks>hopofreq</remarks>
        [Obsolete]
        public long? HopoFreq { get; set; }

        /// <summary>
        /// (FoFiX) Sets the "early hit window" size.
        /// Valid values are "none", "half", or "full".
        /// </summary>
        /// <remarks>early_hit_window_size</remarks>
        public string? EarlyHitWindowSize { get; set; }

        /// <summary>
        /// (CH) Sets whether or not end events in the chart will be respected.
        /// </summary>
        /// <remarks>end_events</remarks>
        public bool? EndEvents { get; set; }

        /// <summary>
        /// (PS) Enables .mid SysEx events for guitar sliders/tap notes.
        /// </summary>
        /// <remarks>sysex_slider</remarks>
        public bool? SysExSlider { get; set; }

        /// <summary>
        /// (PS) Enables .mid SysEx events for Real Drums hi-hat pedal control.
        /// </summary>
        /// <remarks>sysex_high_hat_ctrl</remarks>
        public bool? SysExHighHatCtrl { get; set; }

        /// <summary>
        /// (PS) Enables .mid SysEx events for Real Drums rimshot hits.
        /// </summary>
        /// <remarks>sysex_rimshot</remarks>
        public bool? SysExRimshot { get; set; }

        /// <summary>
        /// (PS) Enables .mid SysEx events for guitar open notes.
        /// </summary>
        /// <remarks>sysex_open_bass</remarks>
        public bool? SysExOpenBass { get; set; }

        /// <summary>
        /// (PS) Enables .mid SysEx events for Pro Guitar/Bass slide directions.
        /// </summary>
        /// <remarks>sysex_pro_slide</remarks>
        public bool? SysExProSlide { get; set; }

        /// <summary>
        /// (PS) Sound sample set index for guitar.
        /// </summary>
        /// <remarks>guitar_type</remarks>
        public long? GuitarType { get; set; }

        /// <summary>
        /// (PS) Sound sample set index for bass.
        /// </summary>
        /// <remarks>bass_type</remarks>
        public long? BassType { get; set; }

        /// <summary>
        /// (PS) Sound sample set index for drums.
        /// </summary>
        /// <remarks>kit_type</remarks>
        public long? KitType { get; set; }

        /// <summary>
        /// (PS) Sound sample set index for keys.
        /// </summary>
        /// <remarks>keys_type</remarks>
        public long? KeysType { get; set; }

        /// <summary>
        /// (PS) Sound sample set index for dance.
        /// </summary>
        /// <remarks>dance_type</remarks>
        public long? DanceType { get; set; }

        #endregion

        #region Images and Other Resources

        /// <summary>
        /// Name of an icon image to display for this song.
        /// Included in either the chart folder or the game the chart was made for, or sourced from this repository of icons.
        /// </summary>
        /// <remarks>icon</remarks>
        public string? Icon { get; set; }

        /// <summary>
        /// Name for a background image file.
        /// </summary>
        /// <remarks>background</remarks>
        public string? Background { get; set; }

        /// <summary>
        /// Name for a background video file.
        /// </summary>
        /// <remarks>video</remarks>
        public string? Video { get; set; }

        /// <summary>
        /// Name for a background video file.
        /// </summary>
        /// <remarks>video_loop</remarks>
        public bool? VideoLoop { get; set; }

        /// <summary>
        /// Timestamp in milliseconds where playback of an included video will start. Can be negative.
        /// This tag controls the time relative to the video, not relative to the chart. Negative values will delay the video, positive values will make the video be at a further point in when the chart starts.
        /// </summary>
        /// <remarks>video_start_time</remarks>
        public long? VideoStartTime { get; set; }

        /// <summary>
        /// Timestamp in milliseconds where playback of an included video will end. -1 means no time is specified.
        /// This is assumed to also be relative to the video, not the chart.
        /// </summary>
        /// <remarks>video_end_time</remarks>
        public long? VideoEndTime { get; set; }

        /// <summary>
        /// Name for a cover image file.
        /// </summary>
        /// <remarks>cover</remarks>
        public string? Cover { get; set; }

        #endregion

        #region Images and Other Resources (Game-Specific)

        /// <summary>
        /// (PS) Name for banner A.
        /// </summary>
        /// <remarks>link_name_a</remarks>
        public string? LinkNameA { get; set; }

        /// <summary>
        /// (PS) Name for banner B.
        /// </summary>
        /// <remarks>link_name_b</remarks>
        public string? LinkNameB { get; set; }

        /// <summary>
        /// (PS) Link that clicking banner A will open.
        /// </summary>
        /// <remarks>banner_link_a</remarks>
        public string? BannerLinkA { get; set; }

        /// <summary>
        /// (PS) Link that clicking banner B will open.
        /// </summary>
        /// <remarks>banner_link_b</remarks>
        public string? BannerLinkB { get; set; }

        #endregion

        #region Miscellaneous (Game-Specific)

        /// <summary>
        /// (FoFiX) High score data.
        /// </summary>
        /// <remarks>scores</remarks>
        public string? Scores { get; set; }

        /// <summary>
        /// (FoFiX) Additional score data.
        /// </summary>
        /// <remarks>scores_ext</remarks>
        public string? ScoresExt { get; set; }

        /// <summary>
        /// (FoFiX) Play count.
        /// </summary>
        /// <remarks>count</remarks>
        public long? Count { get; set; }

        /// <summary>
        /// (PS) Player's rating of the song
        /// </summary>
        /// <remarks>rating</remarks>
        public long? Rating { get; set; }

        /// <summary>
        /// (FoFiX) Career ID for this song.
        /// </summary>
        /// <remarks>unlock_id</remarks>
        public string? UnlockId { get; set; }

        /// <summary>
        /// (FoFiX) The career ID that must be completed to unlock this song.
        /// </summary>
        /// <remarks>unlock_require</remarks>
        public string? UnlockRequire { get; set; }

        /// <summary>
        /// (FoFiX) Text to display if the song is locked.
        /// </summary>
        /// <remarks>unlock_text</remarks>
        public string? UnlockText { get; set; }

        /// <summary>
        /// (FoFiX) Indicates if the song is unlocked.
        /// </summary>
        /// <remarks>unlock_completed</remarks>
        public long? UnlockCompleted { get; set; }

        /// <summary>
        /// (Editor on Fire) Sets a velocity number for drums accent notes.
        /// </summary>
        /// <remarks>eof_midi_import_drum_accent_velocity</remarks>
        public long? EoFMidiImportDrumAccentVelocity { get; set; }

        /// <summary>
        /// (Editor on Fire) Sets a velocity number for drums ghost notes.
        /// </summary>
        /// <remarks>eof_midi_import_drum_ghost_velocity</remarks>
        public long? EoFMidiImportDrumGhostVelocity { get; set; }

        #endregion
    }
}