using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace dichternebel.YaSB.MacroDeckPlug
{
    public class YaSBTransformation : INotifyPropertyChanged
    {
        [ReadOnly(true)]
        public string Variable { get; set; }

        [ReadOnly(true)]
        public string Value { get; set; }

        private string _jsonKey = string.Empty;
        [DisplayName("JSON Key")]
        public string JsonKey
        {
            get => _jsonKey;
            set
            {
                if (value == _jsonKey) return;
                var isValidKey = TrySetKey(value);
                _jsonKey = isValidKey ? value : string.Empty;
                OnPropertyChanged("TransformationValue");
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

                if (string.IsNullOrEmpty(keyValue)) return false;

                var keyParts = keyValue.Split(new[] { '.' }, 2);
                var element = root;

                foreach (var part in keyParts)
                {
                    if (!element.TryGetProperty(part, out var value))
                        return false;
                    element = value;
                }

                return true;
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

                if (string.IsNullOrEmpty(JsonKey))
                    return Value;

                var keyParts = JsonKey.Split(new[] { '.' }, 2);
                var element = root;

                foreach (var part in keyParts)
                {
                    if (!element.TryGetProperty(part, out var value))
                        return Value;
                    element = value;
                }

                return element.ToString();
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
                var keys = new List<string>();

                foreach (var prop in root.EnumerateObject())
                {
                    keys.Add(prop.Name);

                    if (prop.Value.ValueKind == JsonValueKind.Object)
                    {
                        foreach (var childProp in prop.Value.EnumerateObject())
                        {
                            keys.Add($"{prop.Name}.{childProp.Name}");
                        }
                    }
                }

                keys.Insert(0, "<none>");
                return keys;
            }
            catch { }
            return [];
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
