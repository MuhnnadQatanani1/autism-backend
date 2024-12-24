using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

public static class PythonHelper
{
    public static async Task<MemoryStream> GenerateAudioAsync(string text, string language)
    {
        return await Task.Run(() =>
        {
            try
            {
                var start = new ProcessStartInfo
                {
                    FileName = "C:\\Users\\muhnn\\AppData\\Local\\Programs\\Python\\Python313\\python.exe",  // تأكد من أن Python مثبت في البيئة لديك
                    Arguments = $"Models//generate_audio.py \"{text}\" {language}", // مسار الملف Python
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                // تشغيل كود Pythonذ
                using (var process = Process.Start(start))
                {
                    process.WaitForExit();
                }

                // قراءة الملف الصوتي الناتج
                var audioFilePath = "output.wav";
                if (!File.Exists(audioFilePath))
                {
                    throw new FileNotFoundException("لم يتم العثور على الملف الصوتي الناتج.");
                }

                var memoryStream = new MemoryStream(File.ReadAllBytes(audioFilePath));
                memoryStream.Position = 0;
                return memoryStream;
            }
            catch (Exception ex)
            {
                throw new Exception($"خطأ أثناء تشغيل كود Python: {ex.Message}");
            }
        });
    }
}
