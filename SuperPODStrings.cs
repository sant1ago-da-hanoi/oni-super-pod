using System;
using System.Reflection;
using UnityEngine;

namespace SuperPOD
{
    /// <summary>
    /// Registers localized strings for PLib Options UI.
    /// PLib uses parameterless [Option()] which auto-generates keys:
    /// STRINGS.SUPERPOD.OPTIONS.{PROPERTYNAME}.NAME / .TOOLTIP / .CATEGORY
    /// </summary>
    public static class SuperPODStrings
    {
        private const string PREFIX = "STRINGS.SUPERPOD.OPTIONS.";

        public static void Register(bool vietnamese)
        {
            if (vietnamese)
                RegisterVietnamese();
            else
                RegisterEnglish();
        }

        private static void Add(string property, string name, string tooltip, string category)
        {
            string key = PREFIX + property + ".";
            Strings.Add(key + "NAME", name);
            Strings.Add(key + "TOOLTIP", tooltip);
            if (!string.IsNullOrEmpty(category))
                Strings.Add(key + "CATEGORY", category);
        }

        private static void SetPLibString(string fieldName, string value)
        {
            try
            {
                // PLib internalizes types after ILRepack, so search all assemblies
                foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (var type in asm.GetTypes())
                    {
                        if (type.Name == "PLibStrings")
                        {
                            var field = type.GetField(fieldName, BindingFlags.Public | BindingFlags.Static);
                            if (field != null && field.FieldType == typeof(LocString))
                            {
                                field.SetValue(null, new LocString(value));
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[SuperPOD] Failed to set PLib string {fieldName}: {e.Message}");
            }
        }

        private static void RegisterEnglish()
        {
            // Spawn
            Add("TIMEBEFORESPAWN", "Time Before Spawn",
                "Printing Pod seconds per cycle. Game default is 1800.", "Spawn");

            // Blueprint
            Add("DUPLICANTNUMBER", "Duplicant Number",
                "Number of duplicants to choose from (0-10). Duplicant + Care Package <= 10.", "Blueprint");
            Add("CAREPACKAGENUMBER", "Care Package Number",
                "Number of care packages to choose from (0-10). Duplicant + Care Package <= 10.", "Blueprint");

            // Interests
            Add("INTERESTNUMBER", "Interest Number",
                "Number of interests per duplicant (0-13).", "Interests");
            Add("INTERESTVALUE", "Interest Value",
                "Base attribute value for each interest (0-1000). 10% chance of 2x.", "Interests");

            // Traits
            Add("POSITIVETRAITSNUMBER", "Positive Traits",
                "Number of good traits per duplicant.", "Traits");
            Add("NEGATIVETRAITSNUMBER", "Negative Traits",
                "Number of bad traits per duplicant.", "Traits");

            // Stress Reactions
            Add("STRESSAGGRESSIVE", "Destructive",
                "Damages nearby equipment and tiles when stressed.", "Stress Reactions");
            Add("STRESSVOMITER", "Vomiter",
                "Vomits Polluted Water on the floor when stressed.", "Stress Reactions");
            Add("STRESSUGLYCRIER", "Ugly Crier",
                "Sits and cries, producing Water and reducing Decor.", "Stress Reactions");
            Add("STRESSBINGEEATER", "Binge Eater",
                "Eats large amounts of food when stressed.", "Stress Reactions");
            Add("STRESSBANSHEE", "Banshee",
                "Wails loudly, stressing nearby Duplicants.", "Stress Reactions");

            // Overjoyed Responses
            Add("JOYBALLOONARTIST", "Balloon Artist",
                "Hands out balloons that boost skills.", "Overjoyed Responses");
            Add("JOYSPARKLESTREAKER", "Sparkle Streaker",
                "Boosts Athletics for self and nearby Duplicants.", "Overjoyed Responses");
            Add("JOYSTICKERBOMBER", "Sticker Bomber",
                "Places stickers with +20 Decor.", "Overjoyed Responses");
            Add("JOYSUPERPRODUCTIVE", "Super Productive",
                "10% chance to instantly complete chores.", "Overjoyed Responses");
            Add("JOYHAPPYSINGER", "Yodeler",
                "Boosts Machinery, Construction, and Strength.", "Overjoyed Responses");
        }

        private static void RegisterVietnamese()
        {
            // Spawn — Máy in (sinh học)
            Add("TIMEBEFORESPAWN", "Thời gian chờ in",
                "Số giây mỗi chu kỳ của Máy in. Mặc định: 1800.", "Máy in");

            // Blueprint — Bản thiết kế
            Add("DUPLICANTNUMBER", "Số đệ",
                "Số đệ để chọn (0-10). Đệ + Gói <= 10.", "Bản thiết kế");
            Add("CAREPACKAGENUMBER", "Số gói",
                "Số gói để chọn (0-10). Đệ + Gói <= 10.", "Bản thiết kế");

            // Interests — Sở thích
            Add("INTERESTNUMBER", "Số sở thích",
                "Số sở thích mỗi đệ (0-13).", "Sở thích");
            Add("INTERESTVALUE", "Giá trị sở thích",
                "Giá trị thuộc tính cơ bản cho mỗi sở thích (0-1000). 10% cơ hội x2.", "Sở thích");

            // Traits — Phẩm chất
            Add("POSITIVETRAITSNUMBER", "Phẩm chất tốt",
                "Số phẩm chất tốt mỗi đệ.", "Phẩm chất");
            Add("NEGATIVETRAITSNUMBER", "Phẩm chất xấu",
                "Số phẩm chất xấu mỗi đệ.", "Phẩm chất");

            // Stress Reactions — Phản ứng stress
            Add("STRESSAGGRESSIVE", "Phá hoại",
                "Khi bị stress, đệ sẽ xả giận lên công trình và máy móc.", "Phản ứng stress");
            Add("STRESSVOMITER", "Nôn mửa",
                "Khi stress, đệ sẽ nôn mửa khắp nơi.", "Phản ứng stress");
            Add("STRESSUGLYCRIER", "Khóc nhè",
                "Ngồi khóc, tạo Nước và giảm Trang trí.", "Phản ứng stress");
            Add("STRESSBINGEEATER", "Ăn uống vô độ",
                "Ăn lượng lớn thức ăn khi stress.", "Phản ứng stress");
            Add("STRESSBANSHEE", "Càu nhàu",
                "La hét ầm ĩ, gây stress cho đệ gần đó.", "Phản ứng stress");

            // Overjoyed Responses — Phản ứng hưng phấn
            Add("JOYBALLOONARTIST", "Nghệ sĩ bóng bay",
                "Phát bóng bay tăng kỹ năng.", "Phản ứng hưng phấn");
            Add("JOYSPARKLESTREAKER", "Bàn chân lấp lánh",
                "Tăng Thể lực cho bản thân và đệ gần đó.", "Phản ứng hưng phấn");
            Add("JOYSTICKERBOMBER", "Vua hình dán",
                "Dán hình +20 Trang trí.", "Phản ứng hưng phấn");
            Add("JOYSUPERPRODUCTIVE", "Siêu năng suất",
                "10% cơ hội hoàn thành công việc ngay lập tức.", "Phản ứng hưng phấn");
            Add("JOYHAPPYSINGER", "Giọng ca vàng",
                "Đệ sẽ hát khi đạt trạng thái hưng phấn.", "Phản ứng hưng phấn");

            // PLib dialog buttons
            SetPLibString("BUTTON_MANUAL", "CẤU HÌNH THỦ CÔNG");
            SetPLibString("BUTTON_RESET", "ĐẶT LẠI MẶC ĐỊNH");
            SetPLibString("BUTTON_OK", "Xong");
            SetPLibString("BUTTON_OPTIONS", "TÙY CHỌN");
            SetPLibString("MOD_HOMEPAGE", "Trang chủ mod");
            SetPLibString("MOD_VERSION", "Phiên bản: {0}");
            SetPLibString("MOD_ASSEMBLY_VERSION", "Phiên bản assembly: {0}");
            SetPLibString("DIALOG_TITLE", "Tùy chọn {0}");
            SetPLibString("RESTART_REQUIRED", "Cần khởi động lại game để áp dụng thay đổi.");
            SetPLibString("RESTART_OK", "KHỞI ĐỘNG LẠI");
            SetPLibString("RESTART_CANCEL", "TIẾP TỤC");

            // PLib dialog tooltips
            SetPLibString("TOOLTIP_MANUAL", "Mở thư mục chứa file cấu hình đầy đủ.");
            SetPLibString("TOOLTIP_RESET", "Đặt lại cấu hình về giá trị mặc định.");
            SetPLibString("TOOLTIP_OK", "Lưu tùy chọn. Một số thay đổi cần khởi động lại game.");
            SetPLibString("TOOLTIP_CANCEL", "Hủy thay đổi.");
            SetPLibString("TOOLTIP_HOMEPAGE", "Truy cập trang web của mod.");
            SetPLibString("TOOLTIP_VERSION", "Phiên bản hiện tại của mod.\n\nSo sánh với Release Notes để kiểm tra cập nhật.");
            SetPLibString("TOOLTIP_TOGGLE", "Hiện hoặc ẩn nhóm tùy chọn này");
            SetPLibString("TOOLTIP_NEXT", "Tiếp");
            SetPLibString("TOOLTIP_PREVIOUS", "Trước");

            // PLib color picker
            SetPLibString("TOOLTIP_RED", "Đỏ");
            SetPLibString("TOOLTIP_GREEN", "Xanh lá");
            SetPLibString("TOOLTIP_BLUE", "Xanh dương");
            SetPLibString("TOOLTIP_HUE", "Sắc độ");
            SetPLibString("TOOLTIP_SATURATION", "Độ bão hòa");
            SetPLibString("TOOLTIP_VALUE", "Giá trị");

            // PLib outdated warnings
            SetPLibString("OUTDATED_WARNING", "Mod này đã lỗi thời!\nPhiên bản mới: <b>{0}</b>\n\nCập nhật mod thủ công, hoặc dùng <b>Mod Updater</b> để cập nhật mod Steam");
            SetPLibString("OUTDATED_TOOLTIP", "<b><style=\"logic_off\">Đã lỗi thời!</style></b>");
            SetPLibString("MAINMENU_UPDATE", "\n\n<color=#FFCC00>1 mod có thể đã lỗi thời</color>");
            SetPLibString("MAINMENU_UPDATE_1", "\n\n<color=#FFCC00>{0:D} mod có thể đã lỗi thời</color>");

            // PLib key names
            SetPLibString("KEY_CATEGORY_TITLE", "Mod");
            SetPLibString("KEY_ARROWUP", "Mũi tên lên");
            SetPLibString("KEY_ARROWDOWN", "Mũi tên xuống");
            SetPLibString("KEY_ARROWLEFT", "Mũi tên trái");
            SetPLibString("KEY_ARROWRIGHT", "Mũi tên phải");
            SetPLibString("KEY_HOME", "Home");
            SetPLibString("KEY_END", "End");
            SetPLibString("KEY_DELETE", "Delete");
            SetPLibString("KEY_PAGEUP", "Page Up");
            SetPLibString("KEY_PAGEDOWN", "Page Down");
            SetPLibString("KEY_PAUSE", "Pause");
            SetPLibString("KEY_PRTSCREEN", "Print Screen");
            SetPLibString("KEY_SYSRQ", "SysRq");
        }
    }
}
