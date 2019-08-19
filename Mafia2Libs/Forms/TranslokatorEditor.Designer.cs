﻿namespace Mafia2Tool.Forms
{
    partial class TranslokatorEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TranslokatorEditor));
            this.PropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.TranslokatorTree = new System.Windows.Forms.TreeView();
            this.TranslokatorContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.AddInstance = new System.Windows.Forms.ToolStripMenuItem();
            this.AddObject = new System.Windows.Forms.ToolStripMenuItem();
            this.DeleteInstance = new System.Windows.Forms.ToolStripMenuItem();
            this.DeleteObject = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.fileToolButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.SaveToolButton = new System.Windows.Forms.ToolStripMenuItem();
            this.ReloadButton = new System.Windows.Forms.ToolStripMenuItem();
            this.ExitButton = new System.Windows.Forms.ToolStripMenuItem();
            this.CopyButton = new System.Windows.Forms.ToolStripMenuItem();
            this.PasteButton = new System.Windows.Forms.ToolStripMenuItem();
            this.TranslokatorContext.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // PropertyGrid
            // 
            this.PropertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PropertyGrid.Location = new System.Drawing.Point(386, 28);
            this.PropertyGrid.Name = "PropertyGrid";
            this.PropertyGrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.PropertyGrid.Size = new System.Drawing.Size(402, 416);
            this.PropertyGrid.TabIndex = 16;
            // 
            // TranslokatorTree
            // 
            this.TranslokatorTree.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.TranslokatorTree.ContextMenuStrip = this.TranslokatorContext;
            this.TranslokatorTree.HideSelection = false;
            this.TranslokatorTree.Location = new System.Drawing.Point(12, 28);
            this.TranslokatorTree.Name = "TranslokatorTree";
            this.TranslokatorTree.Size = new System.Drawing.Size(368, 416);
            this.TranslokatorTree.TabIndex = 17;
            this.TranslokatorTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TranslokatorTree_AfterSelect);
            this.TranslokatorTree.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnKeyUp);
            // 
            // TranslokatorContext
            // 
            this.TranslokatorContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AddInstance,
            this.AddObject,
            this.DeleteInstance,
            this.DeleteObject,
            this.CopyButton,
            this.PasteButton});
            this.TranslokatorContext.Name = "SDSContext";
            this.TranslokatorContext.Size = new System.Drawing.Size(205, 158);
            this.TranslokatorContext.Opening += new System.ComponentModel.CancelEventHandler(this.TranslokatorContext_Opening);
            // 
            // AddInstance
            // 
            this.AddInstance.Name = "AddInstance";
            this.AddInstance.Size = new System.Drawing.Size(204, 22);
            this.AddInstance.Text = "$ADD_INSTANCE";
            this.AddInstance.Click += new System.EventHandler(this.AddInstance_Click);
            // 
            // AddObject
            // 
            this.AddObject.Name = "AddObject";
            this.AddObject.Size = new System.Drawing.Size(204, 22);
            this.AddObject.Text = "$ADD_OBJECT";
            this.AddObject.Click += new System.EventHandler(this.AddObjectOnClick);
            // 
            // DeleteInstance
            // 
            this.DeleteInstance.Name = "DeleteInstance";
            this.DeleteInstance.ShortcutKeyDisplayString = "DEL";
            this.DeleteInstance.Size = new System.Drawing.Size(204, 22);
            this.DeleteInstance.Text = "$DELETE_INSTANCE";
            this.DeleteInstance.Click += new System.EventHandler(this.DeleteInstance_Click);
            // 
            // DeleteObject
            // 
            this.DeleteObject.Name = "DeleteObject";
            this.DeleteObject.ShortcutKeyDisplayString = "DEL";
            this.DeleteObject.Size = new System.Drawing.Size(204, 22);
            this.DeleteObject.Text = "$DELETE_OBJECT";
            this.DeleteObject.Click += new System.EventHandler(this.DeleteObject_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(800, 25);
            this.toolStrip1.TabIndex = 18;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // fileToolButton
            // 
            this.fileToolButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.fileToolButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SaveToolButton,
            this.ReloadButton,
            this.ExitButton});
            this.fileToolButton.Image = ((System.Drawing.Image)(resources.GetObject("fileToolButton.Image")));
            this.fileToolButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.fileToolButton.Name = "fileToolButton";
            this.fileToolButton.Size = new System.Drawing.Size(47, 22);
            this.fileToolButton.Text = "$FILE";
            // 
            // SaveToolButton
            // 
            this.SaveToolButton.Name = "SaveToolButton";
            this.SaveToolButton.Size = new System.Drawing.Size(124, 22);
            this.SaveToolButton.Text = "$SAVE";
            this.SaveToolButton.Click += new System.EventHandler(this.SaveToolButton_Click);
            // 
            // ReloadButton
            // 
            this.ReloadButton.Name = "ReloadButton";
            this.ReloadButton.Size = new System.Drawing.Size(124, 22);
            this.ReloadButton.Text = "$RELOAD";
            this.ReloadButton.Click += new System.EventHandler(this.ReloadButton_Click);
            // 
            // ExitButton
            // 
            this.ExitButton.Name = "ExitButton";
            this.ExitButton.Size = new System.Drawing.Size(124, 22);
            this.ExitButton.Text = "$EXIT";
            this.ExitButton.Click += new System.EventHandler(this.ExitButton_Click);
            // 
            // CopyButton
            // 
            this.CopyButton.Name = "CopyButton";
            this.CopyButton.Size = new System.Drawing.Size(204, 22);
            this.CopyButton.Text = "$COPY";
            this.CopyButton.Click += new System.EventHandler(this.CopyButton_Click);
            // 
            // PasteButton
            // 
            this.PasteButton.Name = "PasteButton";
            this.PasteButton.Size = new System.Drawing.Size(204, 22);
            this.PasteButton.Text = "$PASTE";
            this.PasteButton.Click += new System.EventHandler(this.PasteButton_Click);
            // 
            // TranslokatorEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.PropertyGrid);
            this.Controls.Add(this.TranslokatorTree);
            this.Controls.Add(this.toolStrip1);
            this.Name = "TranslokatorEditor";
            this.Text = "$TRANSLOKATOR_EDITOR";
            this.TranslokatorContext.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PropertyGrid PropertyGrid;
        private System.Windows.Forms.TreeView TranslokatorTree;
        private System.Windows.Forms.ContextMenuStrip TranslokatorContext;
        private System.Windows.Forms.ToolStripMenuItem AddInstance;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton fileToolButton;
        private System.Windows.Forms.ToolStripMenuItem SaveToolButton;
        private System.Windows.Forms.ToolStripMenuItem ReloadButton;
        private System.Windows.Forms.ToolStripMenuItem ExitButton;
        private System.Windows.Forms.ToolStripMenuItem AddObject;
        private System.Windows.Forms.ToolStripMenuItem DeleteInstance;
        private System.Windows.Forms.ToolStripMenuItem DeleteObject;
        private System.Windows.Forms.ToolStripMenuItem CopyButton;
        private System.Windows.Forms.ToolStripMenuItem PasteButton;
    }
}