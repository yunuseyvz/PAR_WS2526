using UnityEngine;
using System;

namespace LanguageTutor.Utilities
{
    /// <summary>
    /// Utility class for converting WAV byte arrays to Unity AudioClips.
    /// Handles various WAV formats including 8/16/32-bit audio.
    /// </summary>
    public static class WavUtility
    {
        public static AudioClip ToAudioClip(byte[] fileBytes, int offset = 0, string name = "wav")
        {
            // 1. Safety Check: Is this a valid WAV file?
            if (fileBytes == null || fileBytes.Length < 44)
            {
                Debug.LogError("[WavUtility] File is empty or too short.");
                return null;
            }

            // Check for "RIFF" header
            if (fileBytes[0] != 'R' || fileBytes[1] != 'I' || fileBytes[2] != 'F' || fileBytes[3] != 'F')
            {
                string header = System.Text.Encoding.ASCII.GetString(fileBytes, 0, Mathf.Min(100, fileBytes.Length));
                Debug.LogError($"[WavUtility] File is not a WAV. It starts with: '{header}'. (This might be a JSON Error from the server!)");
                return null;
            }

            // 2. Read Header Info
            int channels = BitConverter.ToInt16(fileBytes, 22 + offset);
            int frequency = BitConverter.ToInt32(fileBytes, 24 + offset);
            int bitsPerSample = BitConverter.ToInt16(fileBytes, 34 + offset);

            // 3. Find the "data" chunk (Safe Loop)
            int pos = 12 + offset;
            while (pos + 8 < fileBytes.Length)
            {
                // Check if we found "data" marker (0x64 0x61 0x74 0x61)
                if (fileBytes[pos] == 0x64 && fileBytes[pos + 1] == 0x61 && fileBytes[pos + 2] == 0x74 && fileBytes[pos + 3] == 0x61)
                {
                    break; // Found it!
                }

                // Not "data", so skip this chunk
                pos += 4;
                int chunkSize = BitConverter.ToInt32(fileBytes, pos);
                pos += 4 + chunkSize; // Jump forward
            }

            // 4. Verify we actually found data
            if (pos + 8 >= fileBytes.Length)
            {
                Debug.LogError("[WavUtility] Could not find 'data' chunk in WAV file.");
                return null;
            }

            pos += 8; // Skip "data" + Size bytes to get to the actual audio

            // 5. Convert to Unity AudioClip
            int sampleCount = (fileBytes.Length - pos) / (bitsPerSample / 8);
            if (channels == 2) sampleCount /= 2;

            AudioClip clip = AudioClip.Create(name, sampleCount, channels, frequency, false);
            float[] data = new float[sampleCount * channels];

            int i = 0;
            int step = bitsPerSample / 8; // 2 bytes for 16-bit, 4 bytes for 32-bit

            try
            {
                while (pos + step <= fileBytes.Length)
                {
                    if (bitsPerSample == 16)
                    {
                        // Convert 16-bit int to float [-1, 1]
                        short sh = BitConverter.ToInt16(fileBytes, pos);
                        data[i] = sh / 32768.0f;
                    }
                    else if (bitsPerSample == 32)
                    {
                        // Float 32 is already [-1, 1]
                        data[i] = BitConverter.ToSingle(fileBytes, pos);
                    }
                    else if (bitsPerSample == 8) 
                    {
                        // 8-bit is 0-255, convert to [-1, 1]
                        data[i] = (fileBytes[pos] - 128) / 128.0f;
                    }

                    pos += step;
                    i++;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[WavUtility] Stopped reading early due to file ending: {e.Message}");
            }

            clip.SetData(data, 0);
            return clip;
        }
    }
}
