using System.ComponentModel;

namespace dichternebel.YaSB.MacroDeckPlug
{
    public partial class TransformationsControl : UserControl
    {
        private TableLayoutPanel _table;
        private BindingList<YaSBTransformation> _bindingList;

        public TransformationsControl()
        {
            _table = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                ColumnCount = 4,
                BackColor = Color.FromArgb(33, 33, 33)
            };

            _table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            _table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            _table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            _table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));

            // Headers
            AddHeaderRow();

            Controls.Add(_table);
        }

        private void AddHeaderRow()
        {
            var headers = new[] { "Variable", "Value", "JSON Key", "New Value" };
            for (int i = 0; i < headers.Length; i++)
            {
                var label = new Label
                {
                    Text = headers[i],
                    Font = new Font("Tahoma", 9F, FontStyle.Bold),
                    ForeColor = Color.White,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft
                };
                _table.Controls.Add(label, i, 0);
            }
        }

        public void BindToTransformations(BindingList<YaSBTransformation> yaSBTransformations)
        {
            _bindingList = yaSBTransformations;
            _bindingList.ListChanged += BindingSource_ListChanged;
            RefreshRows();
        }

        private void BindingSource_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType != ListChangedType.ItemChanged) RefreshRows();
        }

        private void RefreshRows()
        {
            if (_bindingList?.Count == null) return;

            // Clear existing data rows
            while (_table.RowCount > 1)
            {
                _table.RowCount--;
            }

            // Add rows for each transformation
            for (int i = 0; i < _bindingList.Count; i++)
            {
                var transformation = _bindingList[i];
                AddDataRow(transformation, i + 1);
            }
        }

        private void AddDataRow(YaSBTransformation transformation, int rowIndex)
        {
            _table.RowCount++;

            var varNameBox = new TextBox
            {
                ReadOnly = true,
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            varNameBox.DataBindings.Add("Text", transformation, nameof(transformation.Variable));

            var varValueBox = new TextBox
            {
                ReadOnly = true,
                Text = transformation.Value,
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            varValueBox.DataBindings.Add("Text", transformation, nameof(transformation.Value));

            Control jsonKeyControl = new Control();

            if (transformation.AvailableKeys?.Any() == true)
            {
                var jsonKeyCombo = new ComboBox
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.FromArgb(45, 45, 45),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    DataSource = transformation.AvailableKeys,
                };
                jsonKeyCombo.DataBindings.Add("SelectedItem", transformation, nameof(transformation.JsonKey));

                // Add binding to keep the JsonKey updated
                jsonKeyCombo.SelectedIndexChanged += (s, e) =>
                {
                    transformation.JsonKey = jsonKeyCombo.SelectedItem?.ToString();
                };
                jsonKeyControl = jsonKeyCombo;
            }
            else
            {
                jsonKeyControl = new TextBox
                {
                    ReadOnly = true,
                    Text = "n.a.",
                    Dock = DockStyle.Fill,
                    BackColor = Color.FromArgb(45, 45, 45),
                    ForeColor = Color.Gray,
                    BorderStyle = BorderStyle.None
                };
            }

            var jsonValueBox = new TextBox
            {
                ReadOnly = true,
                Text = transformation.TransformationValue,
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            jsonValueBox.DataBindings.Add("Text", transformation, nameof(transformation.TransformationValue));

            _table.Controls.Add(varNameBox, 0, rowIndex);
            _table.Controls.Add(varValueBox, 1, rowIndex);
            _table.Controls.Add(jsonKeyControl, 2, rowIndex);
            _table.Controls.Add(jsonValueBox, 3, rowIndex);
        }
    }
}