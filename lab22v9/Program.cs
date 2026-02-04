using System;

namespace lab22v9
{

    // ======================================================
    // 1. ПОЧАТКОВА ІЄРАРХІЯ (ПОРУШЕННЯ LSP)
    // ======================================================

    /*
     Базовий клас MediaPlayer декларує, що будь-який плеєр
     може відтворювати аудіо і відео.
    */
    class MediaPlayer
    {
        public virtual void PlayAudio()
        {
            Console.WriteLine("Відтворення аудіо");
        }

        public virtual void PlayVideo()
        {
            Console.WriteLine("Відтворення відео");
        }
    }

    /*
     AudioOnlyPlayer є похідним класом MediaPlayer,
     але не може відтворювати відео.
     Це порушує контракт базового класу.
    */
    class AudioOnlyPlayer : MediaPlayer
    {
        public override void PlayVideo()
        {
            // Порушення LSP:
            // клієнт очікує, що метод працює, але отримує помилку
            throw new NotSupportedException("Аудіоплеєр не підтримує відео");
        }
    }

    // ======================================================
    // 2. КЛІЄНТСЬКИЙ КОД, ЯКИЙ ДЕМОНСТРУЄ ПРОБЛЕМУ
    // ======================================================

    /*
     Клієнтський метод працює з базовим типом MediaPlayer
     і очікує, що всі його методи будуть коректно виконуватись.
    */
    static class LspViolationDemo
    {
        public static void ClientCode(MediaPlayer player)
        {
            player.PlayAudio();
            player.PlayVideo(); // тут виникає проблема
        }
    }

    // ======================================================
    // 3. АЛЬТЕРНАТИВНЕ LSP-СУМІСНЕ РІШЕННЯ
    // (ЗМІНА ІЄРАРХІЇ)
    // ======================================================

    /*
     Рішення:
     Розділити відповідальності за допомогою інтерфейсів.
     Не всі плеєри повинні підтримувати відео.
    */

    interface IAudioPlayer
    {
        void PlayAudio();
    }

    interface IVideoPlayer
    {
        void PlayVideo();
    }

    /*
     Аудіоплеєр підтримує лише аудіо
    */
    class AudioPlayer : IAudioPlayer
    {
        public void PlayAudio()
        {
            Console.WriteLine("Відтворення аудіо");
        }
    }

    /*
     Відеоплеєр підтримує і аудіо, і відео
    */
    class VideoPlayer : IAudioPlayer, IVideoPlayer
    {
        public void PlayAudio()
        {
            Console.WriteLine("Відтворення аудіо");
        }

        public void PlayVideo()
        {
            Console.WriteLine("Відтворення відео");
        }
    }

    // ======================================================
    // 4. КЛІЄНТСЬКИЙ КОД ДЛЯ НОВОЇ СТРУКТУРИ
    // ======================================================

    static class LspCorrectDemo
    {
        public static void PlayAudioClient(IAudioPlayer player)
        {
            player.PlayAudio();
        }

        public static void PlayVideoClient(IVideoPlayer player)
        {
            player.PlayVideo();
        }
    }

    // ======================================================
    // 5. MAIN — ДЕМОНСТРАЦІЯ РОБОТИ
    // ======================================================

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== ПОРУШЕННЯ LSP ===");

            MediaPlayer audioOnly = new AudioOnlyPlayer();

            try
            {
                LspViolationDemo.ClientCode(audioOnly);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }

            Console.WriteLine();
            Console.WriteLine("=== LSP-СУМІСНЕ РІШЕННЯ ===");

            IAudioPlayer audioPlayer = new AudioPlayer();
            IAudioPlayer videoAsAudio = new VideoPlayer();
            IVideoPlayer videoPlayer = new VideoPlayer();

            LspCorrectDemo.PlayAudioClient(audioPlayer);
            LspCorrectDemo.PlayAudioClient(videoAsAudio);
            LspCorrectDemo.PlayVideoClient(videoPlayer);

            Console.WriteLine();
            Console.WriteLine("Програма завершила роботу коректно.");
        }
    }
}