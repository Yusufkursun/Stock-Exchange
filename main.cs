  using System;
using System.Collections.Generic;

namespace NTPBorsa
{
    public abstract class BaseHisse
    {
        public string HisseAdi { get; set; }
        public decimal Fiyat { get; set; }

        public BaseHisse(string hisseAdi, decimal fiyat)
        {
            HisseAdi = hisseAdi;
            Fiyat = fiyat;
        }

        public abstract void FiyatGuncelle();
    }

    public class Hisse : BaseHisse
    {
        public Hisse(string hisseAdi, decimal fiyat) : base(hisseAdi, fiyat)
        {
        }

        public override void FiyatGuncelle()
        {
            Random rnd = new Random();
            decimal oran = (decimal)(rnd.NextDouble() * 0.2 - 0.1);
            Fiyat += Fiyat * oran;

            if (Fiyat < 0) Fiyat = 0;
        }
    }

    public interface IPortfoy
    {
        void PortfoyGoster(List<BaseHisse> hisseler);
        void HisseSat(BaseHisse hisse, int adet);
    }

    public class Kullanici : IPortfoy
    {
        public decimal Bakiye { get; set; }
        public Dictionary<string, int> Portfoy { get; set; }

        public Kullanici(decimal baslangicBakiye)
        {
            Bakiye = baslangicBakiye;
            Portfoy = new Dictionary<string, int>();
        }

        public void PortfoyGoster(List<BaseHisse> hisseler)
        {
            Console.WriteLine("Portföyünüz:");
            if (Portfoy.Count == 0)
            {
                Console.WriteLine("Portföyünüz boş.");
            }
            else
            {
                decimal toplamDeger = 0;
                foreach (var item in Portfoy)
                {
                    string hisseAdi = item.Key;
                    int adet = item.Value;
                    decimal hisseFiyat = HisseFiyatHesapla(hisseAdi, hisseler);
                    decimal hisseToplamDegeri = hisseFiyat * adet;
                    toplamDeger += hisseToplamDegeri;

                    Console.WriteLine($"{hisseAdi}: {adet} adet, Güncel Fiyat: {hisseFiyat:C2}, Toplam Değer: {hisseToplamDegeri:C2}");
                }
                Console.WriteLine($"Portföydeki toplam değer: {toplamDeger:C2}");
            }
            Console.WriteLine($"Toplam bakiye: {Bakiye:C2}");
            HisseSatimiYap(hisseler);
        }

        private decimal HisseFiyatHesapla(string hisseAdi, List<BaseHisse> hisseler)
        {
            foreach (var hisse in hisseler)
            {
                if (hisse.HisseAdi == hisseAdi)
                {
                    return hisse.Fiyat;
                }
            }
            return 0;
        }

        public void HisseAl(BaseHisse hisse, int adet)
        {
            decimal toplamTutar = hisse.Fiyat * adet;
            if (toplamTutar > Bakiye)
            {
                Console.WriteLine("Bakiyeniz yetersiz!");
            }
            else
            {
                Bakiye -= toplamTutar;
                if (Portfoy.ContainsKey(hisse.HisseAdi))
                    Portfoy[hisse.HisseAdi] += adet;
                else
                    Portfoy.Add(hisse.HisseAdi, adet);

                Console.WriteLine($"{adet} adet {hisse.HisseAdi} aldınız. Kalan bakiye: {Bakiye:C2}");
            }
        }

        public void HisseSat(BaseHisse hisse, int adet)
        {
            if (!Portfoy.ContainsKey(hisse.HisseAdi) || Portfoy[hisse.HisseAdi] < adet)
            {
                Console.WriteLine("Elinizde yeterli hisse yok!");
            }
            else
            {
                Portfoy[hisse.HisseAdi] -= adet;
                if (Portfoy[hisse.HisseAdi] == 0)
                    Portfoy.Remove(hisse.HisseAdi);

                Bakiye += hisse.Fiyat * adet;
                Console.WriteLine($"{adet} adet {hisse.HisseAdi} sattınız. Yeni bakiye: {Bakiye:C2}");
            }
        }

        public void HisseSatimiYap(List<BaseHisse> hisseler)
        {
            Console.WriteLine("1. Satmak istediğiniz hisseyi seçin\n2. Menüye Dön");
            Console.Write("Seçiminizi yapın: ");
            if (int.TryParse(Console.ReadLine(), out int secim))
            {
                switch (secim)
                {
                    case 1:
                        Console.WriteLine("\nSatmak istediğiniz hisseyi seçin:");
                        for (int i = 0; i < hisseler.Count; i++)
                        {
                            if (Portfoy.ContainsKey(hisseler[i].HisseAdi))
                            {
                                Console.WriteLine($"{i + 1}. {hisseler[i].HisseAdi}: {Portfoy[hisseler[i].HisseAdi]} adet");
                            }
                        }

                        Console.Write("Satmak istediğiniz hisseyi seçin (1-3): ");
                        if (int.TryParse(Console.ReadLine(), out int hisseSecimi) && hisseSecimi >= 1 && hisseSecimi <= hisseler.Count)
                        {
                            BaseHisse secilenHisse = hisseler[hisseSecimi - 1];

                            if (Portfoy.ContainsKey(secilenHisse.HisseAdi))
                            {
                                Console.WriteLine("1. Hisseyi Sat\n2. Menüye Dön");
                                Console.Write("Seçiminizi yapın: ");
                                if (int.TryParse(Console.ReadLine(), out int islemSecimi))
                                {
                                    switch (islemSecimi)
                                    {
                                        case 1:
                                            Console.Write("Kaç adet satmak istiyorsunuz? ");
                                            if (int.TryParse(Console.ReadLine(), out int adet))
                                            {
                                                HisseSat(secilenHisse, adet);
                                            }
                                            else
                                            {
                                                Console.WriteLine("Geçersiz adet!");
                                            }
                                            break;

                                        case 2:
                                            Console.WriteLine("Menüye dönülüyor...");
                                            break;

                                        default:
                                            Console.WriteLine("Geçersiz seçim!");
                                            break;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Geçersiz seçim!");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Portföyünüzde bu hisse bulunmuyor!");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Geçersiz seçim!");
                        }
                        break;

                    case 2:
                        Console.WriteLine("Menüye dönülüyor...");
                        break;

                    default:
                        Console.WriteLine("Geçersiz seçim!");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Geçersiz seçim!");
            }
        }


        class Program
        {
            static void Main(string[] args)
            {
                List<BaseHisse> hisseler = new List<BaseHisse>
            {
                new Hisse("HisseA", 100),
                new Hisse("HisseB", 150),
                new Hisse("HisseC", 200)
            };

                Kullanici kullanici = new Kullanici(500);

                while (true)
                {
                    Console.Clear();
                    Console.WriteLine("1. Hisseleri Gör\n2. Portföyü Gör\n3. Çıkış");
                    Console.Write("Seçiminizi yapın: ");
                    int secim = Convert.ToInt32(Console.ReadLine());

                    if (secim == 3)
                    {
                        Console.WriteLine("Çıkış yapılıyor...");
                        break;
                    }

                    switch (secim)
                    {
                        case 1:
                            Console.Clear();
                            Console.WriteLine("Hisseler ve Güncel Fiyatlar:");
                            for (int i = 0; i < hisseler.Count; i++)
                            {
                                Console.WriteLine($"{i + 1}. {hisseler[i].HisseAdi}: {hisseler[i].Fiyat:C2}");
                            }

                            Console.WriteLine($"Bakiye: {kullanici.Bakiye}\n1. Hisse Al\n2. Geri Dön");
                            Console.Write("Seçiminizi yapın: ");
                            int islemSecimi = Convert.ToInt32(Console.ReadLine());

                            if (islemSecimi == 2) break;

                            Console.Write("Hangi hisse ile işlem yapmak istiyorsunuz? (1-3): ");
                            if (int.TryParse(Console.ReadLine(), out int hisseSecimi) && hisseSecimi >= 1 && hisseSecimi <= hisseler.Count)
                            {
                                BaseHisse secilenHisse = hisseler[hisseSecimi - 1];

                                Console.Write("Kaç adet işlem yapmak istiyorsunuz? ");
                                if (int.TryParse(Console.ReadLine(), out int adet))
                                {
                                    kullanici.HisseAl(secilenHisse, adet);
                                }
                                else
                                {
                                    Console.WriteLine("Geçersiz adet!");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Geçersiz seçim!");
                            }
                            Console.ReadKey();
                            break;

                        case 2:
                            Console.Clear();
                            kullanici.PortfoyGoster(hisseler);
                            Console.WriteLine("Devam etmek için bir tuşa basın...");
                            Console.ReadKey();
                            break;

                        default:
                            Console.WriteLine("Geçersiz seçim!");
                            Console.ReadKey();
                            break;
                    }

                    foreach (var hisse in hisseler)
                    {
                        hisse.FiyatGuncelle();
                    }
                }
            }
        }
    }
}
