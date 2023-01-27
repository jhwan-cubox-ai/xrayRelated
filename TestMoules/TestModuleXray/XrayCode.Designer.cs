namespace TestModuleXray
{
    partial class XrayCode
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XrayCode));
            this.Button_HttpJson = new System.Windows.Forms.Button();
            this.StepThreePicture = new System.Windows.Forms.PictureBox();
            this.StepTwoPicture = new System.Windows.Forms.PictureBox();
            this.Button_SendViewer = new System.Windows.Forms.Button();
            this.StepOnePicture = new System.Windows.Forms.PictureBox();
            this.StepOneText = new System.Windows.Forms.TextBox();
            this.StepTwoText = new System.Windows.Forms.TextBox();
            this.StepThreeText = new System.Windows.Forms.TextBox();
            this.StepFourRichText = new System.Windows.Forms.RichTextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.StepFourText = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Button_WinExplorer = new System.Windows.Forms.Button();
            this.Button_NextImage = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.Button_HttpForm = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.StepThreePicture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.StepTwoPicture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.StepOnePicture)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // Button_HttpJson
            // 
            this.Button_HttpJson.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Button_HttpJson.Font = new System.Drawing.Font("굴림", 12F);
            this.Button_HttpJson.Location = new System.Drawing.Point(0, 0);
            this.Button_HttpJson.Name = "Button_HttpJson";
            this.Button_HttpJson.Size = new System.Drawing.Size(152, 40);
            this.Button_HttpJson.TabIndex = 0;
            this.Button_HttpJson.Text = "Json";
            this.Button_HttpJson.UseVisualStyleBackColor = true;
            this.Button_HttpJson.Click += new System.EventHandler(this.Button_HttpJson_Click);
            // 
            // StepThreePicture
            // 
            this.StepThreePicture.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.StepThreePicture.Image = ((System.Drawing.Image)(resources.GetObject("StepThreePicture.Image")));
            this.StepThreePicture.Location = new System.Drawing.Point(527, 483);
            this.StepThreePicture.Name = "StepThreePicture";
            this.StepThreePicture.Size = new System.Drawing.Size(309, 275);
            this.StepThreePicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.StepThreePicture.TabIndex = 2;
            this.StepThreePicture.TabStop = false;
            // 
            // StepTwoPicture
            // 
            this.StepTwoPicture.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.StepTwoPicture.Image = ((System.Drawing.Image)(resources.GetObject("StepTwoPicture.Image")));
            this.StepTwoPicture.Location = new System.Drawing.Point(527, 69);
            this.StepTwoPicture.Name = "StepTwoPicture";
            this.StepTwoPicture.Size = new System.Drawing.Size(309, 275);
            this.StepTwoPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.StepTwoPicture.TabIndex = 1;
            this.StepTwoPicture.TabStop = false;
            // 
            // Button_SendViewer
            // 
            this.Button_SendViewer.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Button_SendViewer.Font = new System.Drawing.Font("굴림", 12F);
            this.Button_SendViewer.Location = new System.Drawing.Point(617, 785);
            this.Button_SendViewer.Name = "Button_SendViewer";
            this.Button_SendViewer.Size = new System.Drawing.Size(128, 41);
            this.Button_SendViewer.TabIndex = 3;
            this.Button_SendViewer.Text = "판독 뷰어 전송";
            this.Button_SendViewer.UseVisualStyleBackColor = true;
            this.Button_SendViewer.Click += new System.EventHandler(this.Button_SendViewer_Click);
            // 
            // StepOnePicture
            // 
            this.StepOnePicture.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.StepOnePicture.Image = ((System.Drawing.Image)(resources.GetObject("StepOnePicture.Image")));
            this.StepOnePicture.Location = new System.Drawing.Point(55, 69);
            this.StepOnePicture.Name = "StepOnePicture";
            this.StepOnePicture.Size = new System.Drawing.Size(309, 275);
            this.StepOnePicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.StepOnePicture.TabIndex = 4;
            this.StepOnePicture.TabStop = false;
            // 
            // StepOneText
            // 
            this.StepOneText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.StepOneText.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.StepOneText.Font = new System.Drawing.Font("굴림", 15F);
            this.StepOneText.Location = new System.Drawing.Point(55, 13);
            this.StepOneText.Name = "StepOneText";
            this.StepOneText.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.StepOneText.Size = new System.Drawing.Size(191, 30);
            this.StepOneText.TabIndex = 6;
            this.StepOneText.Text = "step 1. 촬영 이미지";
            // 
            // StepTwoText
            // 
            this.StepTwoText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.StepTwoText.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.StepTwoText.Font = new System.Drawing.Font("굴림", 15F);
            this.StepTwoText.Location = new System.Drawing.Point(527, 13);
            this.StepTwoText.Name = "StepTwoText";
            this.StepTwoText.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.StepTwoText.Size = new System.Drawing.Size(202, 30);
            this.StepTwoText.TabIndex = 7;
            this.StepTwoText.Text = "step 2. X-ray 이미지";
            // 
            // StepThreeText
            // 
            this.StepThreeText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.StepThreeText.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.StepThreeText.Font = new System.Drawing.Font("굴림", 15F);
            this.StepThreeText.Location = new System.Drawing.Point(527, 427);
            this.StepThreeText.Name = "StepThreeText";
            this.StepThreeText.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.StepThreeText.Size = new System.Drawing.Size(141, 30);
            this.StepThreeText.TabIndex = 8;
            this.StepThreeText.Text = "step 3. AI 모델";
            // 
            // StepFourRichText
            // 
            this.StepFourRichText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.StepFourRichText.Location = new System.Drawing.Point(55, 504);
            this.StepFourRichText.Name = "StepFourRichText";
            this.StepFourRichText.Size = new System.Drawing.Size(309, 234);
            this.StepFourRichText.TabIndex = 9;
            this.StepFourRichText.Text = "";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5.952381F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35.71429F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 17.85714F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35.71429F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.761905F));
            this.tableLayoutPanel1.Controls.Add(this.StepFourRichText, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.StepThreeText, 3, 3);
            this.tableLayoutPanel1.Controls.Add(this.StepOneText, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.StepTwoText, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.StepTwoPicture, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.StepOnePicture, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.StepFourText, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.StepThreePicture, 3, 4);
            this.tableLayoutPanel1.Controls.Add(this.Button_SendViewer, 3, 5);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 3, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.555555F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 38.88889F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.555555F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.555555F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 38.88889F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.555555F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(882, 829);
            this.tableLayoutPanel1.TabIndex = 10;
            // 
            // StepFourText
            // 
            this.StepFourText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.StepFourText.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.StepFourText.Font = new System.Drawing.Font("굴림", 15F);
            this.StepFourText.Location = new System.Drawing.Point(55, 427);
            this.StepFourText.Name = "StepFourText";
            this.StepFourText.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.StepFourText.Size = new System.Drawing.Size(181, 30);
            this.StepFourText.TabIndex = 10;
            this.StepFourText.Text = "step 4.  원격 판독";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.Button_WinExplorer);
            this.panel1.Controls.Add(this.Button_NextImage);
            this.panel1.Location = new System.Drawing.Point(55, 371);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(309, 40);
            this.panel1.TabIndex = 12;
            // 
            // Button_WinExplorer
            // 
            this.Button_WinExplorer.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Button_WinExplorer.Font = new System.Drawing.Font("굴림", 12F);
            this.Button_WinExplorer.Location = new System.Drawing.Point(155, 0);
            this.Button_WinExplorer.Name = "Button_WinExplorer";
            this.Button_WinExplorer.Size = new System.Drawing.Size(154, 40);
            this.Button_WinExplorer.TabIndex = 14;
            this.Button_WinExplorer.Text = "탐색기";
            this.Button_WinExplorer.UseVisualStyleBackColor = true;
            this.Button_WinExplorer.Click += new System.EventHandler(this.button_WinExplorer_Click);
            // 
            // Button_NextImage
            // 
            this.Button_NextImage.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Button_NextImage.Font = new System.Drawing.Font("굴림", 12F);
            this.Button_NextImage.Location = new System.Drawing.Point(3, 0);
            this.Button_NextImage.Name = "Button_NextImage";
            this.Button_NextImage.Size = new System.Drawing.Size(146, 40);
            this.Button_NextImage.TabIndex = 11;
            this.Button_NextImage.Text = "다음 이미지";
            this.Button_NextImage.UseVisualStyleBackColor = true;
            this.Button_NextImage.Click += new System.EventHandler(this.Button_NextImage_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.Button_HttpForm);
            this.panel2.Controls.Add(this.Button_HttpJson);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(527, 371);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(309, 40);
            this.panel2.TabIndex = 13;
            // 
            // Button_HttpForm
            // 
            this.Button_HttpForm.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Button_HttpForm.Font = new System.Drawing.Font("굴림", 12F);
            this.Button_HttpForm.Location = new System.Drawing.Point(158, 0);
            this.Button_HttpForm.Name = "Button_HttpForm";
            this.Button_HttpForm.Size = new System.Drawing.Size(148, 40);
            this.Button_HttpForm.TabIndex = 1;
            this.Button_HttpForm.Text = "Form";
            this.Button_HttpForm.UseVisualStyleBackColor = true;
            this.Button_HttpForm.Click += new System.EventHandler(this.Button_HttpForm_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "Image 파일 (*.jpg, *.png)|*.jpg;*.png|JPG 파일 (*.jpg)|*.jpg|PNG 파일 (*.png)|*.png";
            this.openFileDialog1.InitialDirectory = "\\\\172.16.150.18\\3. NIA X-ray data 검수\\04. 슈퍼브 S3 업로드용 데이터\\20220922_4292\\train";
            this.openFileDialog1.Title = "실제 이미지";
            // 
            // XrayCode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(882, 829);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("굴림", 9F);
            this.Name = "XrayCode";
            this.Text = "센트럴 서버(X-ray 장비)";
            ((System.ComponentModel.ISupportInitialize)(this.StepThreePicture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.StepTwoPicture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.StepOnePicture)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Button_HttpJson;
        private System.Windows.Forms.PictureBox StepThreePicture;
        private System.Windows.Forms.PictureBox StepTwoPicture;
        private System.Windows.Forms.Button Button_SendViewer;
        private System.Windows.Forms.PictureBox StepOnePicture;
        private System.Windows.Forms.TextBox StepOneText;
        private System.Windows.Forms.TextBox StepTwoText;
        private System.Windows.Forms.TextBox StepThreeText;
        private System.Windows.Forms.RichTextBox StepFourRichText;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox StepFourText;
        private System.Windows.Forms.Button Button_NextImage;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button Button_WinExplorer;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button Button_HttpForm;
    }
}

