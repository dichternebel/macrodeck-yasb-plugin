namespace dichternebel.YaSB.MacroDeckPlug
{
    public partial class TransformationsControl : UserControl
    {
        private TableLayoutPanel _table;
        public List<YaSBTransformation> BindingList { get; private set; }

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

        public void BindToTransformations(List<YaSBTransformation> yaSBTransformations)
        {
            BindingList = yaSBTransformations;
            RefreshRows();
        }

        private void RefreshRows()
        {
            // Clear everything
            _table.Controls.Clear();
            _table.RowCount = 0;
            _table.RowStyles.Clear();

            // Leave view empty when there is no data
            if (BindingList?.Count == 0) return;

            // Add header row first
            AddHeaderRow();

            // Add rows for each transformation
            for (int i = 0; i < BindingList.Count; i++)
            {
                var transformation = BindingList[i];
                AddDataRow(transformation, i + 1);
            }
        }

        private void AddDataRow(YaSBTransformation transformation, int rowIndex)
        {
            _table.RowCount++;
            _table.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));

            var varNameBox = CreateTextBoxWithPanel(transformation, nameof(transformation.Variable), true);
            var varValueBox = CreateTextBoxWithPanel(transformation, nameof(transformation.Value), true);
            var jsonValueBox = CreateTextBoxWithPanel(transformation, nameof(transformation.TransformationValue), true);

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
                    ForeColor = Color.LightGray,
                    BorderStyle = BorderStyle.None,
                    Height = 28,
                    Padding = new Padding(3),
                    TextAlign = HorizontalAlignment.Left
                };
            }

            _table.Controls.Add(varNameBox, 0, rowIndex);
            _table.Controls.Add(varValueBox, 1, rowIndex);
            _table.Controls.Add(jsonKeyControl, 2, rowIndex);
            _table.Controls.Add(jsonValueBox, 3, rowIndex);
        }

        private Panel CreateTextBoxWithPanel(YaSBTransformation transformation, string bindingProperty, bool readOnly)
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(33, 33, 33),
                Padding = new Padding(1, 4, 1, 4)
            };

            var textBox = new TextBox
            {
                ReadOnly = readOnly,
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            textBox.DataBindings.Add("Text", transformation, bindingProperty);

            panel.Controls.Add(textBox);
            return panel;
        }
    }
}