# Doğrulama — 20 Ekim 2026

## Güncel veri ve kaynak

- [TFF puan cetveli ve fikstür](https://www.tff.org/Default.aspx?pageID=198) esas alındı: 2026/27 Trendyol Süper Lig Adnan Süvari Sezonu.
- 18 güncel takım, 34 hafta, 306 karşılaşma ve 72 bitmiş sonuç kaynak kayıtlarıyla eşleştirildi.
- 18 takımın O/G/B/M, atılan/yenen gol, averaj, puan ve sırası, maçlardan hesaplanan tablo ile bağımsız TFF tablosu karşılaştırılarak doğrulandı.
- Dört maçta toplam 18 seçilmiş gerçek olay bulunur: 10 gol, 3 sarı kart, 2 kırmızı kart, 3 oyuncu değişikliği. Bunlar maçların tüm olayları değildir.
- Aynı dört maçın ilk/ikinci yarı gol değerleri doğrulandı. Kaynakta bulunmayan topa sahip olma, şut ve korner verileri boş bırakıldı.
- Saati açıklanmayan 162 maç için saat uydurulmadığı; açıklanmamış stadyumun boş kalabildiği kontrol edildi.
- Veri seti dosyası: `League.DataAccessLayer/Data/current-season.json`. Veri tarihi 20 Ekim 2026'dır; otomatik veri güncellemesi yoktur.

## Derleme ve izole kontroller

Bu bölüm teslim öncesinde çalıştırılan kontrollerin kaydıdır. Ayrı doğrulama projesi teslimi sadeleştirmek için sonradan kaldırıldı; aşağıdaki 92 kontrolün sonucu kaldırılmadan önceki çalıştırmaya aittir.

```powershell
dotnet build --no-restore
```

- Derleme: **0 hata, 0 uyarı**.
- SQLite in-memory kontrolleri: **92/92 başarılı**.
- Kontroller; tam fikstürü, bağımsız TFF puan tablosunu, son beş maç hesabını, admin doğrulamalarını, boş istatistikleri, 90+1 uzatmasını, DTO izolasyonunu ve FK/benzersizlik kurallarını kapsar.
- Ayrı bir geçici demo veritabanında eski takım/maç/olay/istatistiklerin güncel sezonla tamamen değiştirildiği test edildi.
- Yeniden açılışın veri çoğaltmadığı ve yöneticinin açıklama değişikliklerini koruduğu doğrulandı.
- Olay güncellemesinin aynı kayıt ve MatchId üzerinde çalıştığı; takım, tür, dakika, uzatma ve açıklamayı kaydettiği test edildi. Yanlış maç/olay kimliği, maç dışındaki takım, geçersiz dakika/uzatma/tür ve boş açıklama reddedilir. Reddedilen güncellemeler mevcut veriyi değiştirmez. Olay düzenlemek skor veya puanı değiştirmez.
- `docs/schema.sql`, iki migration'ı da içeren idempotent SQL Server şema betiği olarak yeniden üretildi.

## Gerçek SQL Server doğrulaması

- Sunucu: `DESKTOP-O7S7PCN`; veritabanı: `LeagueDb`; Windows Authentication.
- Uygulanan migration'lar: `20261020112426_InitialCreate` ve `20261020120130_CurrentSeasonSnapshot`.
- Kullanıcının talebiyle SQL Server yedeği alınmadan eski demo sezonu güncel kayıtlarla değiştirildi.
- Doğrudan SQL sorgusu: **18 takım, 306 maç, 18 olay, 4 istatistik**.
- Durum dağılımı: **72 Bitti, 234 Oynanmadı, 0 Devam ediyor**.
- Olay dağılımı SQL Server'da da **10/3/2/3** olarak doğrulandı.
- `FK_MatchEvents_Matches_MatchId` ve `FK_MatchStatistics_Matches_MatchId` etkin ve güvenilir: `is_disabled=0`, `is_not_trusted=0`.
- MatchEvents için zorunlu çoktan bire, MatchStatistics için zorunlu benzersiz bire bir MatchId ilişkisi korunur. Puan tablosu veya oyuncu tablosu eklenmedi.
- İkinci uygulama açılışında migration ve veri sürümü kontrolü tamamlandı; kayıt sayıları değişmedi.

## HTTP ve admin kontrolleri

Gerçek SQL Server'a bağlı uygulamada aşağıdaki **16 sayfa 200** döndü:

| Sayfa | Kontrol |
|---|---|
| `/` | Güncel sezon özeti |
| `/Home/Teams` | Amed, Erzurumspor FK ve Arca Çorum FK dahil güncel liste |
| `/Home/Standings` | Hesaplanan puan tablosu |
| `/Home/Fixtures?week=9` | Yaklaşan hafta |
| `/Home/Fixtures?week=18` | Saati açıklanmamış program |
| `/Home/Match/55` | Galatasaray – Arca Çorum FK |
| `/Home/Match/60` | Beşiktaş – Eyüpspor |
| `/Home/Match/62` | Amed – Erzurumspor FK; 90+1 gösterimi |
| `/Home/Match/126` | Trabzonspor – Beşiktaş |
| `/Home/Match/215` | Açıklanmayan saat ve stadyum için okunabilir metin |
| `/Admin` | LoginDto ve antiforgery token ile giriş sonrası |
| `/Admin/Create` | Fikstür formu |
| `/Admin/Result/55` | Bilinmeyen topa sahip olma değeri boş |
| `/Admin/Result/126` | Doğrulanmış ilk yarı verileri |
| `/Admin/Result/215` | İstatistiği olmayan maç formu |
| `/Admin/Event/62` | Uzatma dakikası ve kayıtlı olaylar |

Maç kimlikleri mevcut yerel veritabanına aittir; temiz kurulumda farklı olabilir. Sayfalara fikstür kartından ulaşılır. HTTP kontrolleri sırasında skor veya olay eklenmedi/değiştirilmedi.

### Admin olay düzenleme akışı

- Kayıtlı olayın Düzenle bağlantısı, `/Admin/EditEvent/55?eventId=...` adresinde mevcut bilgileri form alanlarına yükledi.
- Geçersiz dakika gönderildiğinde doğrulama hatası gösterildi; formda düzenleme bağlamı korundu.
- Mevcut olayın özgün değerleri POST ile tekrar kaydedildi; başarı mesajıyla olay listesine dönüldü. Olay sayısı artmadı ve kayıt maç detayında göründü.
- Başka maça ait olay için 404; route/form kimlik uyuşmazlığı için 400; antiforgery token olmadan POST için 400 alındı.
- Oturumsuz düzenleme isteği admin girişine yönlendirildi.
- Gerçek lig kaydının değerleri korunarak HTTP kaydetme akışı doğrulandı. Değişen alanların kalıcılığı izole test veritabanında sınandı.
- DTO, servis, controller ve Razor değişiklikleri yapıldı; veritabanı şema değişikliği gerekmedi.

## Sıralama kapsamı

Ödevdeki sadeleştirilmiş puan → averaj → atılan gol → takım adı sıralaması korunur. 20 Ekim 2026 tablosu TFF ile tamamen eşleşir. Sezonun ilerleyen bölümündeki tüm resmî eşitlik kurallarının uygulanacağı iddia edilmez. Gelecek maçların programı TFF tarafından değiştirilebilir.

## Eski yerel veri dosyalarının kaldırılması

- Kullanıcının isteğiyle `League.WebUI/App_Data` klasörü, içindeki eski SQLite veritabanı ve aktarım yedekleri kaldırıldı.
- SQLite aktarım sınıfı, açılıştaki aktarım çağrısı ve SQLite paket başvuruları kaldırıldı. Uygulama yalnızca SQL Server kullanır.
- Temizlik sonrası derleme: **0 hata, 0 uyarı**. SQL Server bağlantısıyla uygulama açıldı; ana sayfa **200** döndü ve `App_Data` yeniden oluşturulmadı.
- Önceki bölümlerdeki SQLite testleri geçmiş doğrulama kayıtlarıdır; güncel uygulamanın bağımlılığı değildir.
