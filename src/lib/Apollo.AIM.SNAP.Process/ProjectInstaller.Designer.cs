namespace Apollo.AIM.SNAP.Process
{
    partial class ProjectInstaller
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SNAPserviceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.SNAPserviceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // SNAPserviceProcessInstaller
            // 
            this.SNAPserviceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.SNAPserviceProcessInstaller.Password = null;
            this.SNAPserviceProcessInstaller.Username = null;
            // 
            // SNAPserviceInstaller
            // 
            this.SNAPserviceInstaller.Description = "Nag SNAP user to approve request";
            this.SNAPserviceInstaller.DisplayName = "SNAP Email Reminder";
            this.SNAPserviceInstaller.ServiceName = "EmailReminder";
            this.SNAPserviceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.SNAPserviceProcessInstaller,
            this.SNAPserviceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller SNAPserviceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller SNAPserviceInstaller;
    }
}