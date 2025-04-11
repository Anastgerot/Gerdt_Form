namespace Gerdt_Form
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.Send = new System.Windows.Forms.Button();
            this.Label = new System.Windows.Forms.Label();
            this.Stop = new System.Windows.Forms.Button();
            this.Start = new System.Windows.Forms.Button();
            this.ListBox = new System.Windows.Forms.ListBox();
            this.TextBox = new System.Windows.Forms.TextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.NumericUpDown = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // Send
            // 
            this.Send.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Send.Location = new System.Drawing.Point(1057, 563);
            this.Send.Margin = new System.Windows.Forms.Padding(6);
            this.Send.Name = "Send";
            this.Send.Size = new System.Drawing.Size(184, 61);
            this.Send.TabIndex = 11;
            this.Send.Text = "Send";
            this.Send.UseVisualStyleBackColor = true;
            this.Send.Click += new System.EventHandler(this.Send_Click);
            // 
            // Label
            // 
            this.Label.AutoSize = true;
            this.Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Label.Location = new System.Drawing.Point(731, 86);
            this.Label.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.Label.Name = "Label";
            this.Label.Size = new System.Drawing.Size(368, 36);
            this.Label.TabIndex = 10;
            this.Label.Text = "Введите кол-во потоков:";
            // 
            // Stop
            // 
            this.Stop.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Stop.Location = new System.Drawing.Point(726, 347);
            this.Stop.Margin = new System.Windows.Forms.Padding(6);
            this.Stop.Name = "Stop";
            this.Stop.Size = new System.Drawing.Size(384, 83);
            this.Stop.TabIndex = 8;
            this.Stop.Text = "Stop";
            this.Stop.UseVisualStyleBackColor = true;
            this.Stop.Click += new System.EventHandler(this.Stop_Click);
            // 
            // Start
            // 
            this.Start.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Start.Location = new System.Drawing.Point(726, 237);
            this.Start.Margin = new System.Windows.Forms.Padding(6);
            this.Start.Name = "Start";
            this.Start.Size = new System.Drawing.Size(384, 81);
            this.Start.TabIndex = 7;
            this.Start.Text = "Start";
            this.Start.UseVisualStyleBackColor = true;
            this.Start.Click += new System.EventHandler(this.Start_Click);
            // 
            // ListBox
            // 
            this.ListBox.FormattingEnabled = true;
            this.ListBox.ItemHeight = 31;
            this.ListBox.Location = new System.Drawing.Point(15, 26);
            this.ListBox.Margin = new System.Windows.Forms.Padding(6);
            this.ListBox.Name = "ListBox";
            this.ListBox.Size = new System.Drawing.Size(584, 655);
            this.ListBox.TabIndex = 6;
            // 
            // TextBox
            // 
            this.TextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.TextBox.Location = new System.Drawing.Point(637, 574);
            this.TextBox.Name = "TextBox";
            this.TextBox.Size = new System.Drawing.Size(398, 41);
            this.TextBox.TabIndex = 12;
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Label1.Location = new System.Drawing.Point(684, 510);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(301, 36);
            this.Label1.TabIndex = 13;
            this.Label1.Text = "Введите сообщение";
            // 
            // NumericUpDown
            // 
            this.NumericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.NumericUpDown.Location = new System.Drawing.Point(670, 149);
            this.NumericUpDown.Name = "NumericUpDown";
            this.NumericUpDown.Size = new System.Drawing.Size(491, 41);
            this.NumericUpDown.TabIndex = 14;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1256, 707);
            this.Controls.Add(this.NumericUpDown);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.TextBox);
            this.Controls.Add(this.Send);
            this.Controls.Add(this.Label);
            this.Controls.Add(this.Stop);
            this.Controls.Add(this.Start);
            this.Controls.Add(this.ListBox);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Send;
        private System.Windows.Forms.Label Label;
        private System.Windows.Forms.Button Stop;
        private System.Windows.Forms.Button Start;
        private System.Windows.Forms.ListBox ListBox;
        private System.Windows.Forms.TextBox TextBox;
        private System.Windows.Forms.Label Label1;
        private System.Windows.Forms.NumericUpDown NumericUpDown;
    }
}

