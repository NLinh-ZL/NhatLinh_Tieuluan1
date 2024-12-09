using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NhatLinh_Tieuluan1
{
    public partial class Audit : Form
    {
        private OracleConnection conn;
        private FlowLayoutPanel flowLayoutPanel;
        public Audit()
        {
            InitializeComponent();
            CenterToScreen();
            conn = Database.Get_Connect();

            this.flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel.Location = new System.Drawing.Point(100, 100);
            this.flowLayoutPanel.Size = new System.Drawing.Size(this.ClientSize.Width - 20,this.ClientSize.Height - 110);
            this.flowLayoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            this.flowLayoutPanel.AutoScroll = true;
            this.flowLayoutPanel.FlowDirection = FlowDirection.TopDown;
            this.flowLayoutPanel.WrapContents = false;
            this.Controls.Add(this.flowLayoutPanel);
        }

        private void load_Cbo_User(OracleConnection conn)
        {
            try
            {
                using (OracleCommand command = new OracleCommand("pro_select_all_users", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    OracleParameter outParam = new OracleParameter("v_out", OracleDbType.RefCursor);
                    outParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(outParam);

                    command.ExecuteNonQuery();

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        cbo_User.Items.Clear();
                        while(reader.Read())
                        {
                            string userName = reader.GetString(0);
                            cbo_User.Items.Add(userName);
                            cbo_User.SelectedIndex = 0;
                        }    
                    }    
                }    
            }
            catch (OracleException ex)
            {
                MessageBox.Show("Erorr select user: " + ex.Message);
            }
        }

        private void Audit_Load(object sender, EventArgs e)
        {
            load_Cbo_User(conn);
        }

        private void cbo_User_SelectedIndexChanged(object sender, EventArgs e)
        {
            string user = cbo_User.SelectedItem.ToString();
            load_dynamic_audit_checkboxes(user, conn);
        }

        private void load_dynamic_audit_checkboxes(string user , OracleConnection conn)
        {
            try
            {
                flowLayoutPanel.Controls.Clear();

                using(OracleCommand command = new OracleCommand("SYS.pro_select_stmt_audit_opts", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("p_user", OracleDbType.Varchar2).Value = user;
                    command.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    using(OracleDataAdapter adapter = new OracleDataAdapter(command))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        HashSet<String> uniqueOptions = new HashSet<string>();

                        foreach(DataRow row in dt.Rows)
                        {
                            string auditOption = row["AUDIT_OPTION"].ToString().ToUpper();
                            uniqueOptions.Add(auditOption);
                        }

                        foreach(string option in uniqueOptions)
                        {
                            CheckBox cb = new CheckBox
                            {
                                Text = option,
                                Checked = true,
                                AutoSize = true
                            };
                            cb.CheckedChanged += CheckBox_CheckedChanged;
                            flowLayoutPanel.Controls.Add(cb);
                        }
                    }
                }

                string[] additionalOptions = {"CREATE ANY TABLE","DROP ANY TABLE","DELETE TABLE",
                                       "INSERT TABLE", "SELECT TABLE", "UPDATE TABLE",
                                       "DELETE ANY TABLE", "INSERT ANY TABLE", "SELECT ANY TABLE", "UPDATE ANY TABLE"};
                            foreach(string option in additionalOptions)
                {
                    if(!flowLayoutPanel.Controls.Cast<CheckBox>().Any(cb=>cb.Text == option))
                    {
                        CheckBox cb = new CheckBox
                        {
                            Text = option,
                            AutoSize = true,
                            Checked = false
                        };
                        cb.CheckedChanged += CheckBox_CheckedChanged;
                        flowLayoutPanel.Controls.Add(cb);
                    }
                }    
            }
            catch (Exception e) 
            {
                MessageBox.Show("Lỗi không xem bảng audit user được: " + e.Message);
            }
        }

        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            string userName = cbo_User.SelectedItem.ToString();
            CheckBox cb = sender as CheckBox;
            if (cb != null) 
            {
                if(cb.Checked) 
                {
                    ExecuteAuditProcedure("pro_create_audit", cb.Text, userName);
                }
                else
                {
                    ExecuteAuditProcedure("pro_drop_audit", cb.Text, userName);
                }
            }
        }

        private void ExecuteAuditProcedure(string procedure, string statement, string username)
        {
            try
            {
                using(OracleCommand cmd = new OracleCommand(procedure, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("p_statement", OracleDbType.Varchar2).Value = statement;
                    cmd.Parameters.Add("p_username", OracleDbType.Varchar2).Value = username;

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Audit command executed successfully for user"+username,
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch(OracleException ex) 
            {
                MessageBox.Show("Erorr executing audit procedure: "+ex.Message,"Erorr", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }
    }
}
