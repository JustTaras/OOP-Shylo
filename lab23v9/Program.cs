using System;

namespace lab23
{
    // =====================================================
    // ПОЧАТКОВА РЕАЛІЗАЦІЯ (ПОРУШЕННЯ ISP ТА DIP)
    // =====================================================

    class TwitterApi
    {
        public void PostToTwitter(string message)
        {
            Console.WriteLine("Twitter: " + message);
        }
    }

    class FacebookApi
    {
        public void PostToFacebook(string message)
        {
            Console.WriteLine("Facebook: " + message);
        }
    }

    class InstagramApi
    {
        public void PostToInstagram(string message)
        {
            Console.WriteLine("Instagram: " + message);
        }
    }

    // Порушує ISP і DIP
    class BadSocialPoster
    {
        private TwitterApi twitter;
        private FacebookApi facebook;
        private InstagramApi instagram;

        public BadSocialPoster()
        {
            twitter = new TwitterApi();
            facebook = new FacebookApi();
            instagram = new InstagramApi();
        }

        public void Post(string message)
        {
            twitter.PostToTwitter(message);
            facebook.PostToFacebook(message);
            instagram.PostToInstagram(message);
        }
    }

    // =====================================================
    // РЕФАКТОРИНГ (ISP + DIP + DI)
    // =====================================================

    // ISP: вузький інтерфейс
    interface ISocialPoster
    {
        void Post(string message);
    }

    class TwitterPoster : ISocialPoster
    {
        public void Post(string message)
        {
            Console.WriteLine("Twitter: " + message);
        }
    }

    class FacebookPoster : ISocialPoster
    {
        public void Post(string message)
        {
            Console.WriteLine("Facebook: " + message);
        }
    }

    class InstagramPoster : ISocialPoster
    {
        public void Post(string message)
        {
            Console.WriteLine("Instagram: " + message);
        }
    }

    // DIP: залежить від інтерфейсу
    // DI: залежність передається через конструктор
    class SocialPoster
    {
        private ISocialPoster poster;

        public SocialPoster(ISocialPoster poster)
        {
            this.poster = poster;
        }

        public void Publish(string message)
        {
            poster.Post(message);
        }
    }

    // =====================================================
    // DEMO
    // =====================================================

    class Program
    {
        static void Main()
        {
            // Поганий варіант
            BadSocialPoster badPoster = new BadSocialPoster();
            badPoster.Post("Повідомлення у всі соцмережі");

            Console.WriteLine();

            // Хороший варіант з DI
            ISocialPoster twitter = new TwitterPoster();
            SocialPoster poster = new SocialPoster(twitter);
            poster.Publish("Повідомлення тільки у Twitter");
        }
    }
}