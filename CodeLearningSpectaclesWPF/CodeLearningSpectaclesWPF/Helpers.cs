using CodeLearningSpectaclesWPF.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace CodeLearningSpectaclesWPF
{
  class Helpers
  {

    internal static AuthObject? Profile { get; set; }
    internal static string? ProfileID { get; set; }
    internal const string CLIENT_ID = "6ab621f34a0c32c827fe";

    internal static string EncryptMessage(string plainText)
    {
      using (Aes aesAlg = Aes.Create())
      {
        aesAlg.Key = Encoding.UTF8.GetBytes("0123456789abcdef0123456789abcdef");
        aesAlg.IV = Encoding.UTF8.GetBytes("1234567890abcdef");

        ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        using (MemoryStream msEncrypt = new MemoryStream())
        {
          using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
          {
            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
            {
              swEncrypt.Write(plainText);
            }
          }
          return Convert.ToBase64String(msEncrypt.ToArray());
        }
      }
    }

    internal static string DecryptMessage(string cipherText)
    {
      using (Aes aesAlg = Aes.Create())
      {
        aesAlg.Key = Encoding.UTF8.GetBytes("0123456789abcdef0123456789abcdef");
        aesAlg.IV = Encoding.UTF8.GetBytes("1234567890abcdef");

        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        byte[] cipherBytes = Convert.FromBase64String(cipherText);

        using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
        {
          using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
          {
            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
            {
              return srDecrypt.ReadToEnd();
            }
          }
        }
      }
    }

    internal static async Task<bool> GetAccessTokenAsync(HttpClient client, DeviceVerification deviceVerification)
    {
      await Task.Delay(5000);
      string url = "https://github.com/login/oauth/access_token?client_id=" + CLIENT_ID + "&device_code=" + deviceVerification.device_code + "&grant_type=urn:ietf:params:oauth:grant-type:device_code";
      HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, url);
      msg.Headers.Add("Accept", "application/json");
      HttpResponseMessage response = await client.SendAsync(msg);

      string content = await response.Content.ReadAsStringAsync();
      AccessToken? accessToken = JsonSerializer.Deserialize<AccessToken>(content);
      if (accessToken != null && accessToken.access_token != string.Empty)
      {
        WriteToFile(accessToken.access_token);
        Environment.SetEnvironmentVariable("ACCESS_TOKEN", accessToken.access_token);
        return true;
      }
      return false;
    }

    internal static void WriteToFile(string message)
    {
      try
      {
        var encrypted = EncryptMessage(message);

        var localPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var complete = System.IO.Path.Combine(localPath, "CodeLearningSpectacles\\Auth.txt");

        StreamWriter sw = new StreamWriter(complete, false, Encoding.Unicode);
        sw.Write(encrypted);
        sw.Close();
      }
      catch (Exception e)
      {
        Console.WriteLine("Exception: " + e.Message);
      }
    }

    internal static string ReadFromFile()
    {
      var localPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
      var completePath = System.IO.Path.Combine(localPath, "CodeLearningSpectacles\\Auth.txt");
      try
      {
        StreamReader sr = new StreamReader(completePath);
        string line = "";
        if (sr.Peek() != -1)
        {
          line = sr.ReadLine();
        }
        sr.Close();
        var test = DecryptMessage(line);
        return test;
      }
      catch (DirectoryNotFoundException)
      {
        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(completePath));
      }
      catch (FileNotFoundException)
      {
        File.Create(completePath).Dispose();
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
      }
      return "";
    }

  }
}
