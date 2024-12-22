using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace dichternebel.YaSB.MacroDeckPlug
{
    public class YaSBTransformation : INotifyPropertyChanged
    {
        private string _jsonKey;

        [ReadOnly(true)]
        public string Variable { get; set; }

        [ReadOnly(true)]
        public string Value { get; set; }

        [DisplayName("JSON Key")]
        public string JsonKey
        {
            get => _jsonKey;
            set
            {
                if (value == _jsonKey) return;
                var isValidKey = TrySetKey(value);
                _jsonKey = isValidKey ? value : string.Empty;
                OnPropertyChanged();
                OnPropertyChanged("TransformationValue");
                Main.Model.SaveTransformations();
            }
        }

        public List<string> AvailableKeys { get => GetAvailableKeys(Value); }

        [ReadOnly(true)]
        [DisplayName("New Yalue")]
        public string TransformationValue { get => GetValueByKey(); }

        private bool TrySetKey(string keyValue)
        {
            try
            {
                using var jsonDocument = JsonDocument.Parse(Value);
                var root = jsonDocument.RootElement;
                var result = false;
                if (!string.IsNullOrEmpty(keyValue) && root.TryGetProperty(keyValue, out var value))
                {
                    result = true;
                }
                return result;
            }
            catch { }
            return false;
        }

        private string GetValueByKey()
        {
            try
            {
                using var jsonDocument = JsonDocument.Parse(Value);
                var root = jsonDocument.RootElement;
                if (!string.IsNullOrEmpty(JsonKey) && root.TryGetProperty(JsonKey, out var value))
                {
                    return value.ToString();
                }
            }
            catch { }
            return Value;
        }

        private List<string> GetAvailableKeys(string jsonString)
        {
            try
            {
                using var jsonDocument = JsonDocument.Parse(jsonString);
                var root = jsonDocument.RootElement;

                var keys = root.EnumerateObject()
                    .Select(prop => prop.Name)
                    .ToList();

                keys.Insert(0, "<none>");
                return keys;
            }
            catch { }
            return new List<string>();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
