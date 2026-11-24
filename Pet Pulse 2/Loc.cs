using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace PetPulse
{
    public static class Loc
    {
        public static readonly string[] LangNames =
            { "Українська", "English", "中文", "Español", "Deutsch", "Français", "Polski", "Italiano", "Türkçe", "日本語" };

        private static readonly string[] Cultures =
            { "uk-UA", "en-US", "zh-CN", "es-ES", "de-DE", "fr-FR", "pl-PL", "it-IT", "tr-TR", "ja-JP" };

        private static readonly Dictionary<string, string> Icons = new Dictionary<string, string>
        {
            ["nav_meds"] = "💊",
            ["nav_docs"] = "📄",
            ["nav_reminders"] = "🔔",
            ["nav_health"] = "❤️",
            ["nav_about"] = "ℹ️",
            ["exit"] = "🚪",
            ["menu_settings"] = "⚙️",
            ["menu_vet"] = "🐾",
            ["meds_title"] = "💊",
            ["health_title"] = "❤️",
            ["rem_title"] = "🔔",
            ["docs_title"] = "📄",
            ["about_title"] = "ℹ️",
            ["delete_sel"] = "🗑️",
            ["reset"] = "🔄",
            ["pet_category"] = "🐕",
            ["today_badge"] = "🔔",
            ["health_tip"] = "💡",
            ["doc_passport"] = "📘",
            ["doc_vaccine"] = "💉",
            ["cat_graphics"] = "🎨",
            ["cat_language"] = "🌐",
            ["cat_notify"] = "🔔",
            ["cat_remarks"] = "💬",
            ["cat_general"] = "🧰",
            ["reset_settings"] = "🔄",
            ["role_switch_to_dev"] = "🔁",
            ["role_switch_to_user"] = "🔁",
            ["remark_add_btn"] = "✍️",
            ["remark_view_btn"] = "📋",
            ["rem_list_title"] = "📋",
            ["rem_add_title"] = "✍️",
            ["vet_title"] = "🐾",
            ["cat"] = "🐱",
            ["dog"] = "🐶",
            ["booked_title"] = "✅"
        };

        private static readonly Dictionary<string, string[]> Table = Parse();

        public static int Index { get; private set; }
        public static CultureInfo Culture => CultureInfo.GetCultureInfo(Cultures[Index]);
        public static event Action? LanguageChanged;

        public static void Init()
        {
            int i = AppSettingsStore.Current.Language;
            Index = i >= 0 && i < LangNames.Length ? i : 0;
            Publish();
        }

        public static void Set(int index)
        {
            if (index < 0 || index >= LangNames.Length) return;
            Index = index;
            AppSettingsStore.Current.Language = index;
            AppSettingsStore.Save();
            Publish();
            LanguageChanged?.Invoke();
        }

        public static string Get(string key)
        {
            if (!Table.TryGetValue(key, out var v)) return key;
            return Index < v.Length ? v[Index] : v[0];
        }

        public static string F(string key, params object[] args) => string.Format(Culture, Get(key), args);

        public static bool IsAny(string key, string text) => Table.TryGetValue(key, out var v) && v.Contains(text);

        private static void Publish()
        {
            foreach (var kv in Table)
            {
                string icon = Icons.TryGetValue(kv.Key, out var ic) ? ic + "  " : "";
                Application.Current.Resources["T_" + kv.Key] = icon + Get(kv.Key);
            }
        }

        private static Dictionary<string, string[]> Parse()
        {
            var d = new Dictionary<string, string[]>();
            foreach (var raw in Data.Split('\n'))
            {
                string line = raw.Trim();
                int eq = line.IndexOf('=');
                if (eq <= 0) continue;
                d[line.Substring(0, eq)] = line.Substring(eq + 1).Split('|');
            }
            return d;
        }

        private const string Data = @"
app_title=PetPulse — Трекер здоров'я улюбленця|PetPulse — Pet Health Tracker|PetPulse — 宠物健康追踪器|PetPulse — Salud de mascotas|PetPulse — Haustier-Gesundheitstracker|PetPulse — Suivi de santé animale|PetPulse — Tracker zdrowia zwierząt|PetPulse — Salute degli animali|PetPulse — Evcil Hayvan Sağlık Takibi|PetPulse — ペットの健康トラッカー
nav_meds=Запис ліків|Medication log|用药记录|Registro de medicinas|Medikamente|Journal des médicaments|Dziennik leków|Registro farmaci|İlaç kaydı|投薬記録
nav_docs=Документи|Documents|文档|Documentos|Dokumente|Documents|Dokumenty|Documenti|Belgeler|書類
nav_reminders=Нагадування|Reminders|提醒|Recordatorios|Erinnerungen|Rappels|Przypomnienia|Promemoria|Hatırlatıcılar|リマインダー
nav_health=Здоров'я та Пульс|Health & Pulse|健康与脉搏|Salud y pulso|Gesundheit & Puls|Santé et pouls|Zdrowie i tętno|Salute e battito|Sağlık ve Nabız|健康と脈拍
nav_about=Про застосунок|About the app|关于应用|Acerca de la app|Über die App|À propos|O aplikacji|Informazioni|Uygulama hakkında|アプリについて
exit=Вихід|Exit|退出|Salir|Beenden|Quitter|Wyjście|Esci|Çıkış|終了
menu_tooltip=Меню|Menu|菜单|Menú|Menü|Menu|Menu|Menu|Menü|メニュー
menu_settings=Налаштування|Settings|设置|Ajustes|Einstellungen|Paramètres|Ustawienia|Impostazioni|Ayarlar|設定
menu_vet=Записатися до ветеринара|Book a vet visit|预约兽医|Pedir cita con el veterinario|Tierarzttermin buchen|Prendre rendez-vous chez le vétérinaire|Umów wizytę u weterynarza|Prenota dal veterinario|Veteriner randevusu al|獣医を予約
meds_title=Графік та запис ліків|Medication schedule & log|用药计划与记录|Calendario y registro de medicinas|Medikamentenplan und -protokoll|Planning et journal des médicaments|Harmonogram i dziennik leków|Programma e registro farmaci|İlaç programı ve kaydı|投薬スケジュールと記録
med_name_ph=Назва ліків|Medicine name|药品名称|Nombre del medicamento|Medikamentenname|Nom du médicament|Nazwa leku|Nome del farmaco|İlaç adı|薬の名前
med_dose_default=1 табл / день|1 tab / day|每天1片|1 comp. / día|1 Tabl. / Tag|1 cp / jour|1 tabl. / dzień|1 cpr / giorno|günde 1 tablet|1日1錠
add_btn=+ Додати|+ Add|+ 添加|+ Añadir|+ Hinzufügen|+ Ajouter|+ Dodaj|+ Aggiungi|+ Ekle|+ 追加
delete_sel=Видалити обране|Delete selected|删除所选|Eliminar selección|Auswahl löschen|Supprimer la sélection|Usuń zaznaczone|Elimina selezionati|Seçileni sil|選択を削除
today=Сьогодні|Today|今天|Hoy|Heute|Aujourd'hui|Dziś|Oggi|Bugün|今日
tomorrow=Завтра|Tomorrow|明天|Mañana|Morgen|Demain|Jutro|Domani|Yarın|明日
seed_med1_name=Вітаміни Canina V20|Canina V20 vitamins|Canina V20 维生素|Vitaminas Canina V20|Canina V20 Vitamine|Vitamines Canina V20|Witaminy Canina V20|Vitamine Canina V20|Canina V20 vitaminleri|Canina V20 ビタミン
seed_med1_dose=1 таблетка вранці|1 tablet in the morning|早上1片|1 comprimido por la mañana|1 Tablette morgens|1 comprimé le matin|1 tabletka rano|1 compressa al mattino|sabah 1 tablet|朝1錠
seed_med2_name=Краплі Симпаріка|Simparica drops|Simparica 滴剂|Gotas Simparica|Simparica-Tropfen|Gouttes Simparica|Krople Simparica|Gocce Simparica|Simparica damlası|シンパリカ滴下薬
seed_med2_dose=1 крапля|1 drop|1滴|1 gota|1 Tropfen|1 goutte|1 kropla|1 goccia|1 damla|1滴
health_title=Моніторинг здоров'я та показників|Health & vitals monitoring|健康与指标监测|Monitoreo de salud y signos vitales|Gesundheits- und Vitalwerte|Suivi de la santé et des constantes|Monitoring zdrowia i parametrów|Monitoraggio di salute e parametri|Sağlık ve vital takip|健康・バイタル監視
pet_category=Категорія тварини:|Pet category:|宠物类别：|Categoría de mascota:|Tierkategorie:|Catégorie d'animal :|Kategoria zwierzęcia:|Categoria animale:|Evcil hayvan kategorisi:|ペットの区分：
breed_small=Дрібна порода (Шпіц, Чихуахуа: 1 - 5 кг)|Small breed (Spitz, Chihuahua: 1–5 kg)|小型犬（博美、吉娃娃：1–5公斤）|Raza pequeña (Spitz, Chihuahua: 1–5 kg)|Kleine Rasse (Spitz, Chihuahua: 1–5 kg)|Petite race (Spitz, Chihuahua : 1–5 kg)|Mała rasa (szpic, chihuahua: 1–5 kg)|Razza piccola (Spitz, Chihuahua: 1–5 kg)|Küçük ırk (Spitz, Chihuahua: 1–5 kg)|小型犬（スピッツ、チワワ：1〜5kg）
breed_mid=Середня порода (Коргі, Бігль: 5 - 15 кг)|Medium breed (Corgi, Beagle: 5–15 kg)|中型犬（柯基、比格：5–15公斤）|Raza mediana (Corgi, Beagle: 5–15 kg)|Mittlere Rasse (Corgi, Beagle: 5–15 kg)|Race moyenne (Corgi, Beagle : 5–15 kg)|Średnia rasa (corgi, beagle: 5–15 kg)|Razza media (Corgi, Beagle: 5–15 kg)|Orta ırk (Corgi, Beagle: 5–15 kg)|中型犬（コーギー、ビーグル：5〜15kg）
breed_large=Велика порода (Лабрадор, Вівчарка: 15 - 40 кг)|Large breed (Labrador, Shepherd: 15–40 kg)|大型犬（拉布拉多、牧羊犬：15–40公斤）|Raza grande (Labrador, Pastor: 15–40 kg)|Große Rasse (Labrador, Schäferhund: 15–40 kg)|Grande race (Labrador, Berger : 15–40 kg)|Duża rasa (labrador, owczarek: 15–40 kg)|Razza grande (Labrador, Pastore: 15–40 kg)|Büyük ırk (Labrador, Çoban köpeği: 15–40 kg)|大型犬（ラブラドール、シェパード：15〜40kg）
reset=Скинути|Reset|重置|Restablecer|Zurücksetzen|Réinitialiser|Resetuj|Ripristina|Sıfırla|リセット
bpm=уд/хв|bpm|次/分|lpm|S/min|bpm|ud./min|bpm|atım/dk|回/分
kg=кг|kg|公斤|kg|kg|kg|kg|kg|kg|kg
health_tip=Показники адаптуються залежно від розміру собаки. Норми пульсу: Дрібні (90–140), Середні (70–120), Великі (60–100).|The readings adapt to the dog's size. Normal pulse: small (90–140), medium (70–120), large (60–100).|指标会根据狗的体型自动调整。正常脉搏：小型（90–140）、中型（70–120）、大型（60–100）。|Los valores se adaptan al tamaño del perro. Pulso normal: pequeño (90–140), mediano (70–120), grande (60–100).|Die Werte passen sich der Hundegröße an. Normaler Puls: klein (90–140), mittel (70–120), groß (60–100).|Les valeurs s'adaptent à la taille du chien. Pouls normal : petit (90–140), moyen (70–120), grand (60–100).|Wartości dostosowują się do rozmiaru psa. Prawidłowe tętno: małe (90–140), średnie (70–120), duże (60–100).|I valori si adattano alla taglia del cane. Battito normale: piccolo (90–140), medio (70–120), grande (60–100).|Değerler köpeğin boyutuna göre uyarlanır. Normal nabız: küçük (90–140), orta (70–120), büyük (60–100).|数値は犬の大きさに合わせて変わります。正常な脈拍：小型（90〜140）、中型（70〜120）、大型（60〜100）。
pulse_norm=Норма ({0}–{1})|Normal ({0}–{1})|正常（{0}–{1}）|Normal ({0}–{1})|Normal ({0}–{1})|Normal ({0}–{1})|Norma ({0}–{1})|Normale ({0}–{1})|Normal ({0}–{1})|正常（{0}〜{1}）
pulse_low=Нижче норми (Нагляд)|Below normal (monitor)|低于正常（观察）|Por debajo de lo normal (vigilar)|Unter dem Normalwert (beobachten)|Sous la normale (surveiller)|Poniżej normy (obserwować)|Sotto la norma (monitorare)|Normalin altında (izleyin)|基準より低い（経過観察）
pulse_lower=Помітно нижче (Увага)|Clearly low (attention)|明显偏低（注意）|Claramente bajo (atención)|Deutlich zu niedrig (Achtung)|Nettement bas (attention)|Wyraźnie niskie (uwaga)|Nettamente basso (attenzione)|Belirgin düşük (dikkat)|かなり低い（注意）
pulse_vlow=Дуже низький (Ветлікар!)|Very low (see a vet!)|非常低（请就医！）|Muy bajo (¡veterinario!)|Sehr niedrig (Tierarzt!)|Très bas (vétérinaire !)|Bardzo niskie (weterynarz!)|Molto basso (veterinario!)|Çok düşük (veteriner!)|非常に低い（獣医へ！）
pulse_high=Вище норми (Тахікардія)|Above normal (tachycardia)|高于正常（心动过速）|Por encima de lo normal (taquicardia)|Über dem Normalwert (Tachykardie)|Au-dessus de la normale (tachycardie)|Powyżej normy (tachykardia)|Sopra la norma (tachicardia)|Normalin üzerinde (taşikardi)|基準より高い（頻脈）
w_obese=Ожиріння (Вище норми)|Obesity (above normal)|肥胖（高于正常）|Obesidad (por encima de lo normal)|Fettleibigkeit (über dem Normalwert)|Obésité (au-dessus de la normale)|Otyłość (powyżej normy)|Obesità (sopra la norma)|Obezite (normalin üzerinde)|肥満（基準より重い）
w_ok=Вага в нормі|Weight is normal|体重正常|Peso normal|Gewicht normal|Poids normal|Waga w normie|Peso nella norma|Kilo normal|体重は正常
w_def_warn=Дефіцит ваги (Увага)|Underweight (attention)|体重不足（注意）|Bajo peso (atención)|Untergewicht (Achtung)|Insuffisance pondérale (attention)|Niedowaga (uwaga)|Sottopeso (attenzione)|Düşük kilo (dikkat)|体重不足（注意）
w_crit=Критичний дефіцит!|Critically underweight!|严重体重不足！|¡Bajo peso crítico!|Kritisches Untergewicht!|Insuffisance pondérale critique !|Krytyczna niedowaga!|Sottopeso critico!|Kritik düşük kilo!|深刻な体重不足！
w_def_low=Дефіцит ваги (Нижче норми)|Underweight (below normal)|体重不足（低于正常）|Bajo peso (por debajo de lo normal)|Untergewicht (unter dem Normalwert)|Insuffisance pondérale (sous la normale)|Niedowaga (poniżej normy)|Sottopeso (sotto la norma)|Düşük kilo (normalin altında)|体重不足（基準より軽い）
t_norm=Норма (37.5–39.0)|Normal (37.5–39.0)|正常（37.5–39.0）|Normal (37.5–39.0)|Normal (37.5–39.0)|Normale (37.5–39.0)|Norma (37.5–39.0)|Normale (37.5–39.0)|Normal (37.5–39.0)|正常（37.5〜39.0）
t_low=Знижена темп. (Увага)|Low temp. (attention)|体温偏低（注意）|Temp. baja (atención)|Niedrige Temp. (Achtung)|Temp. basse (attention)|Niska temp. (uwaga)|Temp. bassa (attenzione)|Düşük ateş (dikkat)|低体温（注意）
t_crit=Критична температура!|Critical temperature!|体温危急！|¡Temperatura crítica!|Kritische Temperatur!|Température critique !|Krytyczna temperatura!|Temperatura critica!|Kritik sıcaklık!|危険な体温！
rem_title=Менеджер нагадувань|Reminder manager|提醒管理|Gestor de recordatorios|Erinnerungsverwaltung|Gestionnaire de rappels|Menedżer przypomnień|Gestione promemoria|Hatırlatıcı yöneticisi|リマインダー管理
rem_ph=Текст нагадування...|Reminder text...|提醒内容…|Texto del recordatorio…|Erinnerungstext …|Texte du rappel…|Treść przypomnienia…|Testo del promemoria…|Hatırlatıcı metni…|リマインダーの内容…
add_rem=+ Нагадати|+ Remind|+ 提醒|+ Recordar|+ Erinnern|+ Rappeler|+ Przypomnij|+ Ricorda|+ Hatırlat|+ 追加
today_badge=Сьогодні!|Today!|今天！|¡Hoy!|Heute!|Aujourd'hui !|Dziś!|Oggi!|Bugün!|今日！
sched_today=Заплановано на сьогодні|Scheduled for today|计划在今天|Programado para hoy|Für heute geplant|Prévu pour aujourd'hui|Zaplanowano na dziś|Programmato per oggi|Bugün için planlandı|今日の予定
sched_on=Заплановано на {0}|Scheduled for {0}|计划在{0}|Programado para {0}|Geplant für {0}|Prévu pour {0}|Zaplanowano na {0}|Programmato per {0}|{0} için planlandı|{0}の予定
toggle_on=УВІМК|ON|开|ON|AN|ON|WŁ.|ON|AÇIK|オン
toggle_off=ВИМК|OFF|关|OFF|AUS|OFF|WYŁ.|OFF|KAPALI|オフ
r_med=Прийом ліків: {0} ({1})|Medicine: {0} ({1})|服药：{0}（{1}）|Medicamento: {0} ({1})|Medikament: {0} ({1})|Médicament : {0} ({1})|Lek: {0} ({1})|Farmaco: {0} ({1})|İlaç: {0} ({1})|投薬：{0}（{1}）
r_visit_planned=Запланований візит до ветеринара|Scheduled vet visit|已安排的兽医就诊|Visita al veterinario programada|Geplanter Tierarztbesuch|Visite chez le vétérinaire prévue|Zaplanowana wizyta u weterynarza|Visita veterinaria programmata|Planlanan veteriner ziyareti|予定されている獣医の診察
r_visit_booked=Візит до ветеринара: {0}, {1}|Vet visit: {0}, {1}|兽医就诊：{0}，{1}|Visita al veterinario: {0}, {1}|Tierarztbesuch: {0}, {1}|Visite chez le vétérinaire : {0}, {1}|Wizyta u weterynarza: {0}, {1}|Visita dal veterinario: {0}, {1}|Veteriner ziyareti: {0}, {1}|獣医の診察：{0}、{1}
docs_title=Паспорт та ветеринарні документи|Passport & veterinary documents|护照与兽医文件|Pasaporte y documentos veterinarios|Pass und Tierarztdokumente|Passeport et documents vétérinaires|Paszport i dokumenty weterynaryjne|Passaporto e documenti veterinari|Pasaport ve veteriner belgeleri|パスポートと獣医書類
doc_passport=Міжнародний ветпаспорт|International pet passport|国际宠物护照|Pasaporte internacional de mascota|Internationaler Heimtierausweis|Passeport international pour animaux|Międzynarodowy paszport dla zwierząt|Passaporto internazionale per animali|Uluslararası evcil hayvan pasaportu|国際ペットパスポート
doc_scan=[ Скан паспорта прикріплено ]|[ Passport scan attached ]|[ 已附护照扫描件 ]|[ Escaneo del pasaporte adjunto ]|[ Passscan angehängt ]|[ Scan du passeport joint ]|[ Skan paszportu dołączony ]|[ Scansione del passaporto allegata ]|[ Pasaport taraması eklendi ]|[ パスポートのスキャン添付済み ]
doc_chip=Примітка: Чіп №968000010293|Note: Chip No. 968000010293|备注：芯片编号 968000010293|Nota: chip n.º 968000010293|Hinweis: Chip-Nr. 968000010293|Remarque : puce n° 968000010293|Uwaga: chip nr 968000010293|Nota: microchip n. 968000010293|Not: Çip No. 968000010293|メモ：マイクロチップ番号 968000010293
doc_vaccine=Сертифікат вакцинації|Vaccination certificate|疫苗接种证书|Certificado de vacunación|Impfzertifikat|Certificat de vaccination|Certyfikat szczepień|Certificato di vaccinazione|Aşı sertifikası|ワクチン接種証明書
doc_vaccine_name=Вакцина: Nobivac DHPPi + RL|Vaccine: Nobivac DHPPi + RL|疫苗：Nobivac DHPPi + RL|Vacuna: Nobivac DHPPi + RL|Impfstoff: Nobivac DHPPi + RL|Vaccin : Nobivac DHPPi + RL|Szczepionka: Nobivac DHPPi + RL|Vaccino: Nobivac DHPPi + RL|Aşı: Nobivac DHPPi + RL|ワクチン：Nobivac DHPPi + RL
doc_photo=[ Фото довідки Nobivac ]|[ Nobivac certificate photo ]|[ Nobivac 证明照片 ]|[ Foto del certificado Nobivac ]|[ Foto des Nobivac-Nachweises ]|[ Photo du certificat Nobivac ]|[ Zdjęcie zaświadczenia Nobivac ]|[ Foto del certificato Nobivac ]|[ Nobivac belgesi fotoğrafı ]|[ Nobivac証明書の写真 ]
doc_valid=Дійсно до: 15.05.2027|Valid until: 15.05.2027|有效期至：2027年5月15日|Válido hasta: 15.05.2027|Gültig bis: 15.05.2027|Valable jusqu'au : 15.05.2027|Ważne do: 15.05.2027|Valido fino al: 15.05.2027|Geçerlilik: 15.05.2027|有効期限：2027年5月15日 about_title=Про застосунок PetPulse|About PetPulse|关于 PetPulse|Acerca de PetPulse|Über PetPulse|À propos de PetPulse|O aplikacji PetPulse|Informazioni su PetPulse|PetPulse hakkında|PetPulseについて
about_text=PetPulse — ваш надійний помічник у догляді за улюбленцями.|PetPulse is your reliable helper for caring for your pets.|PetPulse 是您照顾宠物的可靠助手。|PetPulse es tu ayudante de confianza para cuidar de tus mascotas.|PetPulse ist Ihr zuverlässiger Helfer bei der Pflege Ihrer Haustiere.|PetPulse est votre assistant de confiance pour prendre soin de vos animaux.|PetPulse to Twój niezawodny pomocnik w opiece nad zwierzakami.|PetPulse è il tuo assistente affidabile per la cura dei tuoi animali.|PetPulse, evcil hayvanlarınızın bakımında güvenilir yardımcınızdır.|PetPulseは、ペットのお世話を支える頼れるアシスタントです。
about_version=Версія 1.2.0 · НАУ «ХАІ» 2026|Version 1.2.0 · NAU KhAI 2026|版本 1.2.0 · NAU KhAI 2026|Versión 1.2.0 · NAU KhAI 2026|Version 1.2.0 · NAU KhAI 2026|Version 1.2.0 · NAU KhAI 2026|Wersja 1.2.0 · NAU KhAI 2026|Versione 1.2.0 · NAU KhAI 2026|Sürüm 1.2.0 · NAU KhAI 2026|バージョン 1.2.0 · NAU KhAI 2026
exit_confirm=Ви дійсно бажаєте вийти з застосунку PetPulse?|Do you really want to exit PetPulse?|确定要退出 PetPulse 吗？|¿Seguro que quieres salir de PetPulse?|Möchten Sie PetPulse wirklich beenden?|Voulez-vous vraiment quitter PetPulse ?|Czy na pewno chcesz zamknąć PetPulse?|Vuoi davvero uscire da PetPulse?|PetPulse'tan çıkmak istediğinize emin misiniz?|本当にPetPulseを終了しますか？
notify_text=Сповіщення: на сьогодні заплановано прийом ліків «{0}»!|Notification: “{0}” is scheduled for today!|通知：今天需要服用“{0}”！|Aviso: hoy toca administrar “{0}”.|Hinweis: Für heute ist „{0}“ eingeplant!|Notification : « {0} » est prévu aujourd'hui !|Powiadomienie: na dziś zaplanowano lek „{0}”!|Notifica: oggi è previsto il farmaco «{0}»!|Bildirim: bugün “{0}” ilacı planlandı!|通知：今日は「{0}」の投薬予定です！
notify_title=PetPulse Нагадування|PetPulse Reminder|PetPulse 提醒|Recordatorio de PetPulse|PetPulse-Erinnerung|Rappel PetPulse|Przypomnienie PetPulse|Promemoria PetPulse|PetPulse Hatırlatıcısı|PetPulseリマインダー
set_window=Налаштування — PetPulse|Settings — PetPulse|设置 — PetPulse|Ajustes — PetPulse|Einstellungen — PetPulse|Paramètres — PetPulse|Ustawienia — PetPulse|Impostazioni — PetPulse|Ayarlar — PetPulse|設定 — PetPulse
cat_graphics=Графіка|Graphics|图形|Gráficos|Grafik|Graphismes|Grafika|Grafica|Grafikler|グラフィック
cat_language=Мова|Language|语言|Idioma|Sprache|Langue|Język|Lingua|Dil|言語
cat_notify=Сповіщення|Notifications|通知|Notificaciones|Benachrichtigungen|Notifications|Powiadomienia|Notifiche|Bildirimler|通知
cat_remarks=Зауваження|Remarks|意见|Observaciones|Anmerkungen|Remarques|Uwagi|Osservazioni|Notlar|指摘事項
cat_general=Загальні|General|常规|General|Allgemein|Général|Ogólne|Generali|Genel|一般
pick_section=Оберіть розділ ліворуч|Choose a section on the left|请在左侧选择一个分区|Elige una sección a la izquierda|Wählen Sie links einen Bereich|Choisissez une section à gauche|Wybierz sekcję po lewej|Scegli una sezione a sinistra|Soldan bir bölüm seçin|左側でセクションを選んでください
light_theme=Світла тема|Light theme|浅色主题|Tema claro|Helles Design|Thème clair|Jasny motyw|Tema chiaro|Açık tema|ライトテーマ
light_theme_desc=Увімкніть, щоб застосунок став світлим. Вимкніть — повернеться темне оформлення.|Turn on to make the app light. Turn off to return to the dark look.|开启后应用变为浅色，关闭后恢复深色外观。|Actívalo para que la app sea clara. Desactívalo para volver al aspecto oscuro.|Einschalten für ein helles Erscheinungsbild, ausschalten für das dunkle Design.|Activez pour passer l'application en clair. Désactivez pour revenir au thème sombre.|Włącz, aby aplikacja była jasna. Wyłącz, aby wrócić do ciemnego wyglądu.|Attivalo per rendere l'app chiara. Disattivalo per tornare all'aspetto scuro.|Uygulamayı açık yapmak için açın, koyu görünüme dönmek için kapatın.|オンにするとアプリが明るくなります。オフにすると暗い表示に戻ります。
lang_title=Мова інтерфейсу|Interface language|界面语言|Idioma de la interfaz|Sprache der Oberfläche|Langue de l'interface|Język interfejsu|Lingua dell'interfaccia|Arayüz dili|表示言語
lang_desc=Оберіть мову застосунку. Зміна застосовується одразу й запам'ятовується.|Choose the app language. The change applies immediately and is remembered.|选择应用语言。更改立即生效并会被记住。|Elige el idioma de la app. El cambio se aplica al instante y se recuerda.|Wählen Sie die Sprache der App. Die Änderung gilt sofort und wird gespeichert.|Choisissez la langue de l'application. Le changement s'applique immédiatement et est mémorisé.|Wybierz język aplikacji. Zmiana działa od razu i jest zapamiętywana.|Scegli la lingua dell'app. La modifica è immediata e viene ricordata.|Uygulama dilini seçin. Değişiklik hemen uygulanır ve hatırlanır.|アプリの言語を選びます。変更はすぐに反映され、保存されます。
notify_set_title=Сповіщення про ліки на сьогодні|Reminders for today's medicine|今日用药提醒|Avisos de medicinas de hoy|Hinweise zu heutigen Medikamenten|Alertes pour les médicaments du jour|Powiadomienia o lekach na dziś|Avvisi per i farmaci di oggi|Bugünkü ilaç bildirimleri|今日の投薬通知
notify_set_desc=Показувати спливаюче вікно, коли ви додаєте ліки на сьогоднішній день.|Show a pop-up when you add medicine for today.|添加今天的药品时显示弹窗。|Mostrar una ventana emergente al añadir medicinas para hoy.|Ein Popup anzeigen, wenn Sie Medikamente für heute hinzufügen.|Afficher une fenêtre lorsque vous ajoutez un médicament pour aujourd'hui.|Pokazuj okno, gdy dodajesz lek na dziś.|Mostra un popup quando aggiungi un farmaco per oggi.|Bugün için ilaç eklediğinizde açılır pencere göster.|今日の薬を追加したときにポップアップを表示します。
confirm_exit_title=Підтверджувати вихід|Confirm exit|退出前确认|Confirmar al salir|Beenden bestätigen|Confirmer la sortie|Potwierdzaj wyjście|Conferma l'uscita|Çıkışı onayla|終了時に確認
confirm_exit_desc=Запитувати підтвердження перед закриттям застосунку.|Ask for confirmation before closing the app.|关闭应用前请求确认。|Pedir confirmación antes de cerrar la app.|Vor dem Schließen der App nachfragen.|Demander confirmation avant de fermer l'application.|Pytaj o potwierdzenie przed zamknięciem aplikacji.|Chiedi conferma prima di chiudere l'app.|Uygulamayı kapatmadan önce onay iste.|アプリを閉じる前に確認します。
reset_settings=Скинути налаштування|Reset settings|重置设置|Restablecer ajustes|Einstellungen zurücksetzen|Réinitialiser les paramètres|Zresetuj ustawienia|Ripristina impostazioni|Ayarları sıfırla|設定をリセット
role_title=Режим роботи|Work mode|工作模式|Modo de trabajo|Arbeitsmodus|Mode de travail|Tryb pracy|Modalità di lavoro|Çalışma modu|動作モード
role_user=Користувач|User|用户|Usuario|Benutzer|Utilisateur|Użytkownik|Utente|Kullanıcı|ユーザー
role_dev=Розробник|Developer|开发者|Desarrollador|Entwickler|Développeur|Deweloper|Sviluppatore|Geliştirici|開発者
role_desc_user=Користувач залишає зауваження щодо застосунку.|The user leaves remarks about the app.|用户可对应用提出意见。|El usuario deja observaciones sobre la app.|Der Benutzer hinterlässt Anmerkungen zur App.|L'utilisateur laisse des remarques sur l'application.|Użytkownik zostawia uwagi o aplikacji.|L'utente lascia osservazioni sull'app.|Kullanıcı uygulama hakkında not bırakır.|ユーザーはアプリへの指摘を残します。
role_desc_dev=Розробник переглядає зауваження та відмічає виконані.|The developer reviews the remarks and marks them as done.|开发者查看意见并标记已完成项。|El desarrollador revisa las observaciones y marca las hechas.|Der Entwickler prüft die Anmerkungen und hakt erledigte ab.|Le développeur consulte les remarques et marque celles traitées.|Deweloper przegląda uwagi i oznacza wykonane.|Lo sviluppatore esamina le osservazioni e segna quelle completate.|Geliştirici notları inceler ve tamamlananları işaretler.|開発者は指摘を確認し、対応済みにチェックを付けます。
role_current=Поточний режим: {0}|Current mode: {0}|当前模式：{0}|Modo actual: {0}|Aktueller Modus: {0}|Mode actuel : {0}|Bieżący tryb: {0}|Modalità attuale: {0}|Geçerli mod: {0}|現在のモード：{0}
role_switch_to_dev=Перейти в режим розробника|Switch to developer mode|切换到开发者模式|Cambiar a modo desarrollador|Zum Entwicklermodus wechseln|Passer en mode développeur|Przełącz na tryb dewelopera|Passa alla modalità sviluppatore|Geliştirici moduna geç|開発者モードに切り替え
role_switch_to_user=Перейти в режим користувача|Switch to user mode|切换到用户模式|Cambiar a modo usuario|Zum Benutzermodus wechseln|Passer en mode utilisateur|Przełącz na tryb użytkownika|Passa alla modalità utente|Kullanıcı moduna geç|ユーザーモードに切り替え
role_restart_msg=Застосунок буде перезапущено, а це вікно відкриється знову. Продовжити?|The app will restart and this window will open again. Continue?|应用将重新启动，并再次打开此窗口。是否继续？|La app se reiniciará y esta ventana se abrirá de nuevo. ¿Continuar?|Die App wird neu gestartet und dieses Fenster erneut geöffnet. Fortfahren?|L'application va redémarrer et cette fenêtre s'ouvrira de nouveau. Continuer ?|Aplikacja zostanie uruchomiona ponownie, a to okno otworzy się znowu. Kontynuować?|L'app verrà riavviata e questa finestra si aprirà di nuovo. Continuare?|Uygulama yeniden başlatılacak ve bu pencere tekrar açılacak. Devam edilsin mi?|アプリを再起動し、このウィンドウをもう一度開きます。続行しますか？
remarks_user_text=Опишіть, що в застосунку незручно або що можна покращити. Зауваження буде збережено в списку.|Describe what is inconvenient in the app or what could be improved. The remark will be saved to the list.|请描述应用中不方便或可改进之处，意见将保存到列表中。|Describe qué resulta incómodo o qué se puede mejorar. La observación se guardará en la lista.|Beschreiben Sie, was unpraktisch ist oder verbessert werden kann. Die Anmerkung wird in der Liste gespeichert.|Décrivez ce qui est peu pratique ou ce qui peut être amélioré. La remarque sera ajoutée à la liste.|Opisz, co jest niewygodne lub co można poprawić. Uwaga zostanie zapisana na liście.|Descrivi cosa è scomodo o cosa si può migliorare. L'osservazione verrà salvata nell'elenco.|Uygulamada neyin rahatsız edici olduğunu veya neyin iyileştirilebileceğini yazın. Not listeye kaydedilir.|使いにくい点や改善できる点を書いてください。指摘は一覧に保存されます。
remarks_dev_text=Список зауважень до застосунку. Відмічайте виконані — вони стануть сірими, а галочку можна зняти, щоб повернути зауваження.|The list of remarks about the app. Tick the ones that are done and they turn grey; untick to bring a remark back.|应用意见列表。勾选已完成项，它们会变灰；取消勾选可恢复意见。|Lista de observaciones sobre la app. Marca las hechas y se volverán grises; desmarca para recuperarlas.|Liste der Anmerkungen zur App. Erledigte abhaken, dann werden sie grau; Haken entfernen, um sie zurückzuholen.|Liste des remarques sur l'application. Cochez celles qui sont traitées pour les griser ; décochez pour les rétablir.|Lista uwag do aplikacji. Zaznacz wykonane, a staną się szare; odznacz, aby je przywrócić.|Elenco delle osservazioni sull'app. Spunta quelle fatte e diventano grigie; togli la spunta per ripristinarle.|Uygulamayla ilgili notların listesi. Tamamlananları işaretleyin, gri olurlar; işareti kaldırınca geri döner.|アプリへの指摘の一覧です。対応済みにチェックを付けると灰色になり、外すと元に戻ります。
remark_add_btn=Залишити зауваження|Leave a remark|提交意见|Dejar una observación|Anmerkung hinterlassen|Laisser une remarque|Dodaj uwagę|Lascia un'osservazione|Not bırak|指摘を残す
remark_view_btn=Переглянути зауваження|View remarks|查看意见|Ver observaciones|Anmerkungen ansehen|Voir les remarques|Zobacz uwagi|Vedi osservazioni|Notları görüntüle|指摘を見る
remark_counts=Відкрито: {0} · Виконано: {1}|Open: {0} · Done: {1}|未完成：{0} · 已完成：{1}|Abiertas: {0} · Hechas: {1}|Offen: {0} · Erledigt: {1}|Ouvertes : {0} · Traitées : {1}|Otwarte: {0} · Wykonane: {1}|Aperte: {0} · Fatte: {1}|Açık: {0} · Tamamlanan: {1}|未対応：{0} · 対応済み：{1}
remark_saved=Зауваження збережено.|Remark saved.|意见已保存。|Observación guardada.|Anmerkung gespeichert.|Remarque enregistrée.|Uwaga zapisana.|Osservazione salvata.|Not kaydedildi.|指摘を保存しました。
rem_list_title=Зауваження до застосунку|Remarks about the app|应用意见|Observaciones sobre la app|Anmerkungen zur App|Remarques sur l'application|Uwagi do aplikacji|Osservazioni sull'app|Uygulama notları|アプリへの指摘
rem_open_hint=Натисніть на зауваження, щоб побачити докладний опис.|Click a remark to see the full description.|点击意见查看详细说明。|Pulsa una observación para ver la descripción completa.|Klicken Sie auf eine Anmerkung für die ausführliche Beschreibung.|Cliquez sur une remarque pour voir la description complète.|Kliknij uwagę, aby zobaczyć pełny opis.|Fai clic su un'osservazione per vedere la descrizione completa.|Ayrıntılı açıklamayı görmek için bir nota tıklayın.|指摘をクリックすると詳しい説明が表示されます。
rem_add_title=Нове зауваження|New remark|新意见|Nueva observación|Neue Anmerkung|Nouvelle remarque|Nowa uwaga|Nuova osservazione|Yeni not|新しい指摘
rem_field_title=КОРОТКИЙ ЗАГОЛОВОК|SHORT TITLE|简短标题|TÍTULO CORTO|KURZER TITEL|TITRE COURT|KRÓTKI TYTUŁ|TITOLO BREVE|KISA BAŞLIK|短いタイトル
rem_field_details=ДОКЛАДНИЙ ОПИС|FULL DESCRIPTION|详细说明|DESCRIPCIÓN COMPLETA|AUSFÜHRLICHE BESCHREIBUNG|DESCRIPTION COMPLÈTE|PEŁNY OPIS|DESCRIZIONE COMPLETA|AYRINTILI AÇIKLAMA|詳細な説明
rem_field_author=АВТОР|AUTHOR|作者|AUTOR|AUTOR|AUTEUR|AUTOR|AUTORE|YAZAR|投稿者
rem_save=Зберегти|Save|保存|Guardar|Speichern|Enregistrer|Zapisz|Salva|Kaydet|保存
rem_cancel=Скасувати|Cancel|取消|Cancelar|Abbrechen|Annuler|Anuluj|Annulla|İptal|キャンセル
close=Закрити|Close|关闭|Cerrar|Schließen|Fermer|Zamknij|Chiudi|Kapat|閉じる
rem_err_title=Вкажіть короткий заголовок.|Enter a short title.|请输入简短标题。|Escribe un título corto.|Geben Sie einen kurzen Titel ein.|Saisissez un titre court.|Podaj krótki tytuł.|Inserisci un titolo breve.|Kısa bir başlık girin.|短いタイトルを入力してください。
rem_err_details=Опишіть зауваження докладніше.|Describe the remark in more detail.|请更详细地描述意见。|Describe la observación con más detalle.|Beschreiben Sie die Anmerkung ausführlicher.|Décrivez la remarque plus en détail.|Opisz uwagę dokładniej.|Descrivi meglio l'osservazione.|Notu daha ayrıntılı açıklayın.|指摘をもう少し詳しく書いてください。
status_open=Відкрито|Open|未完成|Abierta|Offen|Ouverte|Otwarte|Aperta|Açık|未対応
status_done=Виконано|Done|已完成|Hecha|Erledigt|Traitée|Wykonane|Fatta|Tamamlandı|対応済み
added=Додано|Added|添加时间|Añadida|Hinzugefügt|Ajoutée|Dodano|Aggiunta|Eklendi|追加日vet_window=Запис до ветеринара — PetPulse|Vet booking — PetPulse|预约兽医 — PetPulse|Cita veterinaria — PetPulse|Tierarzttermin — PetPulse|Rendez-vous vétérinaire — PetPulse|Wizyta u weterynarza — PetPulse|Visita veterinaria — PetPulse|Veteriner randevusu — PetPulse|獣医の予約 — PetPulse
vet_title=Запис до ветеринара|Book a vet visit|预约兽医|Cita con el veterinario|Tierarzttermin|Rendez-vous chez le vétérinaire|Wizyta u weterynarza|Visita dal veterinario|Veteriner randevusu|獣医の予約
step1=Крок 1 з 3 — оберіть клініку|Step 1 of 3 — choose a clinic|第1步（共3步）—选择诊所|Paso 1 de 3: elige una clínica|Schritt 1 von 3 – Klinik wählen|Étape 1 sur 3 — choisissez une clinique|Krok 1 z 3 — wybierz klinikę|Passaggio 1 di 3: scegli una clinica|Adım 1/3 — klinik seçin|ステップ1/3：クリニックを選択
step2=Крок 2 з 3 — оберіть дату й час|Step 2 of 3 — choose date and time|第2步（共3步）—选择日期和时间|Paso 2 de 3: elige fecha y hora|Schritt 2 von 3 – Datum und Uhrzeit wählen|Étape 2 sur 3 — choisissez la date et l'heure|Krok 2 z 3 — wybierz datę i godzinę|Passaggio 2 di 3: scegli data e ora|Adım 2/3 — tarih ve saat seçin|ステップ2/3：日付と時間を選択
step3=Крок 3 з 3 — розкажіть про тварину|Step 3 of 3 — tell us about your pet|第3步（共3步）—填写宠物信息|Paso 3 de 3: cuéntanos sobre tu mascota|Schritt 3 von 3 – Angaben zum Tier|Étape 3 sur 3 — parlez-nous de votre animal|Krok 3 z 3 — opowiedz o zwierzaku|Passaggio 3 di 3: parlaci del tuo animale|Adım 3/3 — evcil hayvanınızı anlatın|ステップ3/3：ペットについて入力
step_done=Готово|Done|完成|Listo|Fertig|Terminé|Gotowe|Fatto|Tamam|完了
clinics_hint=10 клінік поруч — від найближчої. Прокручуйте колесиком миші та натисніть на потрібну.|10 clinics nearby, from the closest. Scroll with the mouse wheel and click the one you need.|附近10家诊所（由近到远）。用鼠标滚轮滚动并点击所需诊所。|10 clínicas cercanas, de la más próxima. Desplázate con la rueda del ratón y pulsa la que necesites.|10 Kliniken in der Nähe, von der nächsten. Mit dem Mausrad scrollen und die gewünschte anklicken.|10 cliniques à proximité, de la plus proche. Faites défiler avec la molette et cliquez sur celle qu'il vous faut.|10 klinik w pobliżu, od najbliższej. Przewijaj kółkiem myszy i kliknij wybraną.|10 cliniche vicine, dalla più vicina. Scorri con la rotellina e fai clic su quella che ti serve.|Yakındaki 10 klinik, en yakından başlayarak. Fare tekerleğiyle kaydırın ve istediğinize tıklayın.|近くのクリニック10件（近い順）。マウスホイールでスクロールして選んでください。
demo_note=Демо-дані: список клінік навчальний, відстані умовні.|Demo data: the clinic list is for training, distances are approximate.|演示数据：诊所列表仅供学习，距离为示意值。|Datos de demostración: la lista de clínicas es didáctica y las distancias son aproximadas.|Demodaten: Die Klinikliste dient Lernzwecken, die Entfernungen sind ungefähr.|Données de démonstration : la liste de cliniques est fictive et les distances approximatives.|Dane demonstracyjne: lista klinik jest edukacyjna, odległości są orientacyjne.|Dati dimostrativi: l'elenco delle cliniche è didattico e le distanze sono indicative.|Demo verisi: klinik listesi eğitim amaçlıdır, mesafeler yaklaşıktır.|デモデータ：クリニック一覧は学習用で、距離は目安です。
km=км|km|公里|km|km|km|km|km|km|km
reviews=відгуків|reviews|条评价|opiniones|Bewertungen|avis|opinii|recensioni|yorum|件の口コミ
date_label=ДАТА|DATE|日期|FECHA|DATUM|DATE|DATA|DATA|TARİH|日付
time_label=ЧАС ПРИЙОМУ|APPOINTMENT TIME|就诊时间|HORA DE LA CITA|TERMINZEIT|HEURE DU RENDEZ-VOUS|GODZINA WIZYTY|ORARIO DELLA VISITA|RANDEVU SAATİ|診察時間
slot_legend=Світлі кнопки — вільний час, зелена — ваш вибір, блякла — вже заброньовано або минуло.|Light buttons are free times, green is your choice, faded means booked or already past.|浅色按钮为空闲时间，绿色为您的选择，灰色表示已被预约或已过期。|Los botones claros son horas libres, el verde es tu elección y el atenuado está reservado o ya pasó.|Helle Schaltflächen sind freie Zeiten, grün ist Ihre Wahl, blass bedeutet vergeben oder vorbei.|Les boutons clairs sont libres, le vert est votre choix, estompé = réservé ou passé.|Jasne przyciski to wolne terminy, zielony to Twój wybór, blady — zajęty lub już minął.|I pulsanti chiari sono orari liberi, il verde è la tua scelta, sbiadito = prenotato o già passato.|Açık düğmeler boş saatler, yeşil seçiminiz, soluk olanlar dolu ya da geçmiş saatlerdir.|明るいボタンは空き時間、緑はあなたの選択、薄いものは予約済みまたは過去の時間です。
slot_busy=вже заброньовано|already booked|已被预约|ya reservado|bereits gebucht|déjà réservé|już zarezerwowane|già prenotato|zaten dolu|すでに予約済み
slot_past=цей час уже минув|this time has already passed|该时间已过|esa hora ya pasó|diese Zeit ist bereits vorbei|cette heure est déjà passée|ta godzina już minęła|questo orario è già passato|bu saat geçti|この時間はすでに過ぎています
slot_pick_other=Оберіть інший час.|Please choose another time.|请选择其他时间。|Elige otra hora.|Bitte wählen Sie eine andere Zeit.|Choisissez un autre horaire.|Wybierz inną godzinę.|Scegli un altro orario.|Başka bir saat seçin.|別の時間を選んでください。
slot_chosen=Обрано: {0} о {1}|Selected: {0} at {1}|已选择：{0} {1}|Seleccionado: {0} a las {1}|Gewählt: {0} um {1}|Sélectionné : {0} à {1}|Wybrano: {0} o {1}|Selezionato: {0} alle {1}|Seçildi: {0}, {1}|選択：{0} {1}
slot_taken_msg=Цей час уже заброньовано. Оберіть інший.|This time is already booked. Choose another.|该时间已被预约，请选择其他时间。|Esa hora ya está reservada. Elige otra.|Diese Zeit ist bereits gebucht. Wählen Sie eine andere.|Cet horaire est déjà réservé. Choisissez-en un autre.|Ta godzina jest już zajęta. Wybierz inną.|Questo orario è già prenotato. Scegline un altro.|Bu saat zaten dolu. Başka bir saat seçin.|この時間はすでに予約済みです。別の時間を選んでください。
day_today_fmt=Сьогодні, {0}|Today, {0}|今天，{0}|Hoy, {0}|Heute, {0}|Aujourd'hui, {0}|Dziś, {0}|Oggi, {0}|Bugün, {0}|今日、{0}
day_tomorrow_fmt=Завтра, {0}|Tomorrow, {0}|明天，{0}|Mañana, {0}|Morgen, {0}|Demain, {0}|Jutro, {0}|Domani, {0}|Yarın, {0}|明日、{0}
f_pet=КЛИЧКА ТВАРИНИ|PET'S NAME|宠物名字|NOMBRE DE LA MASCOTA|NAME DES TIERS|NOM DE L'ANIMAL|IMIĘ ZWIERZAKA|NOME DELL'ANIMALE|EVCİL HAYVANIN ADI|ペットの名前
f_species=ВИД|SPECIES|种类|ESPECIE|TIERART|ESPÈCE|GATUNEK|SPECIE|TÜR|種類
cat=Кіт|Cat|猫|Gato|Katze|Chat|Kot|Gatto|Kedi|猫
dog=Собака|Dog|狗|Perro|Hund|Chien|Pies|Cane|Köpek|犬
f_owner=ІМ'Я ВЛАСНИКА|OWNER'S NAME|主人姓名|NOMBRE DEL DUEÑO|NAME DES BESITZERS|NOM DU PROPRIÉTAIRE|IMIĘ WŁAŚCICIELA|NOME DEL PROPRIETARIO|SAHİBİNİN ADI|飼い主の名前
f_phone=НОМЕР ТЕЛЕФОНУ|PHONE NUMBER|电话号码|TELÉFONO|TELEFONNUMMER|NUMÉRO DE TÉLÉPHONE|NUMER TELEFONU|NUMERO DI TELEFONO|TELEFON NUMARASI|電話番号
f_complaint=ЩО ТУРБУЄ ТВАРИНУ|WHAT IS BOTHERING YOUR PET|宠物有什么不适|QUÉ LE PREOCUPA A LA MASCOTA|WAS DAS TIER BELASTET|CE QUI PRÉOCCUPE L'ANIMAL|CO DOLEGA ZWIERZAKA|COSA PREOCCUPA L'ANIMALE|EVCİL HAYVANI NE RAHATSIZ EDİYOR|気になる症状
err_pet=Вкажіть кличку тварини.|Enter the pet's name.|请输入宠物名字。|Escribe el nombre de la mascota.|Geben Sie den Namen des Tiers ein.|Saisissez le nom de l'animal.|Podaj imię zwierzaka.|Inserisci il nome dell'animale.|Evcil hayvanın adını girin.|ペットの名前を入力してください。
err_species=Оберіть вид тварини: кіт чи собака.|Choose the species: cat or dog.|请选择种类：猫或狗。|Elige la especie: gato o perro.|Wählen Sie die Tierart: Katze oder Hund.|Choisissez l'espèce : chat ou chien.|Wybierz gatunek: kot lub pies.|Scegli la specie: gatto o cane.|Türü seçin: kedi veya köpek.|種類を選んでください：猫または犬。
err_owner=Вкажіть ім'я власника.|Enter the owner's name.|请输入主人姓名。|Escribe el nombre del dueño.|Geben Sie den Namen des Besitzers ein.|Saisissez le nom du propriétaire.|Podaj imię właściciela.|Inserisci il nome del proprietario.|Sahibinin adını girin.|飼い主の名前を入力してください。
err_phone=Вкажіть коректний номер телефону (10–13 цифр).|Enter a valid phone number (10–13 digits).|请输入有效的电话号码（10–13位数字）。|Escribe un teléfono válido (10–13 dígitos).|Geben Sie eine gültige Telefonnummer ein (10–13 Ziffern).|Saisissez un numéro valide (10 à 13 chiffres).|Podaj poprawny numer telefonu (10–13 cyfr).|Inserisci un numero valido (10–13 cifre).|Geçerli bir telefon numarası girin (10–13 hane).|有効な電話番号を入力してください（10〜13桁）。
booked_title=Запис підтверджено!|Booking confirmed!|预约已确认！|¡Cita confirmada!|Termin bestätigt!|Rendez-vous confirmé !|Wizyta potwierdzona!|Prenotazione confermata!|Randevu onaylandı!|予約が確定しました！
booked_note=Нагадування про візит додано в розділ «Нагадування».|The visit reminder was added to the Reminders section.|就诊提醒已添加到“提醒”栏目。|El recordatorio de la visita se añadió a Recordatorios.|Die Erinnerung wurde im Bereich „Erinnerungen“ hinzugefügt.|Le rappel de la visite a été ajouté à la section Rappels.|Przypomnienie o wizycie dodano do sekcji Przypomnienia.|Il promemoria della visita è stato aggiunto ai Promemoria.|Ziyaret hatırlatıcısı Hatırlatıcılar bölümüne eklendi.|診察のリマインダーを「リマインダー」に追加しました。
d_clinic=Клініка|Clinic|诊所|Clínica|Klinik|Clinique|Klinika|Clinica|Klinik|クリニック
d_address=Адреса|Address|地址|Dirección|Adresse|Adresse|Adres|Indirizzo|Adres|住所
d_datetime=Дата й час|Date and time|日期和时间|Fecha y hora|Datum und Uhrzeit|Date et heure|Data i godzina|Data e ora|Tarih ve saat|日時
d_pet=Тварина|Pet|宠物|Mascota|Tier|Animal|Zwierzę|Animale|Evcil hayvan|ペット
d_owner=Власник|Owner|主人|Dueño|Besitzer|Propriétaire|Właściciel|Proprietario|Sahip|飼い主
d_phone_short=тел.|tel.|电话|tel.|Tel.|tél.|tel.|tel.|tel.|電話
d_complaint=Що турбує|Concern|不适情况|Motivo|Anliegen|Motif|Dolegliwości|Motivo|Şikâyet|症状
not_given=не вказано|not specified|未填写|no indicado|nicht angegeben|non précisé|nie podano|non indicato|belirtilmedi|未記入
back=← Назад|← Back|← 返回|← Atrás|← Zurück|← Retour|← Wstecz|← Indietro|← Geri|← 戻る
next=Далі →|Next →|下一步 →|Siguiente →|Weiter →|Suivant →|Dalej →|Avanti →|İleri →|次へ →
book_btn=✔ Записатися|✔ Book|✔ 预约|✔ Reservar|✔ Buchen|✔ Réserver|✔ Zapisz się|✔ Prenota|✔ Randevu al|✔ 予約する
";
    }
}