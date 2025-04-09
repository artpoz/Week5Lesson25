using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Week5Lesson25
{
    public partial class AddEditStudent : Form
    {
        private string _filePath = Program.FilePath;

        private int _studentId;
        private Student _student;
        private List<Group> _groups;

        private FileHelper<List<Student>> _fileHelper =
            new FileHelper<List<Student>>(Program.FilePath);

        public AddEditStudent(int id = 0)
        {
            InitializeComponent();
            _studentId = id;

            _groups = new List<Group>();
            _groups.Add(new Group { Id = 0, Name = "None" });
            _groups.Add(new Group { Id = 1, Name = "A1" });
            _groups.Add(new Group { Id = 2, Name = "A2" });
            _groups.Add(new Group { Id = 3, Name = "A3" });

            cbGroupId.DataSource = _groups;
            cbGroupId.DisplayMember = "Name";
            cbGroupId.ValueMember = "Id";

            GetStudentData();          

            tbName.Select();                        
        }        

        private void GetStudentData()
        {
            if (_studentId != 0)
            {
                Text = "Edytowanie danych ucznia";

                var students = _fileHelper.DeserializeFromFile();
                _student = students.FirstOrDefault(x => x.Id == _studentId);

                if (_student == null)
                    throw new Exception("Brak użytkownika o podanym Id");

                FillTextBoxes();
            }
        }

        private void FillTextBoxes()
        {
            tbId.Text = _student.Id.ToString();
            tbName.Text = _student.FirstName;
            tbLastName.Text = _student.LastName;
            tbMath.Text = _student.Math;
            tbPhysics.Text = _student.Physics;
            tbTechnology.Text = _student.Technology;
            rtbComments.Text = _student.Comments;
            tbPolish.Text = _student.PolishLang;
            tbForeign.Text = _student.ForeignLang;
            cbExtra.Checked = _student.Extra;
            cbGroupId.SelectedItem = _groups.FirstOrDefault(x => x.Id == _student.GroupId);
        }
        
        private void btnConfirm_Click(object sender, EventArgs e)
        {
            var students = _fileHelper.DeserializeFromFile();

            if (_studentId != 0)            
                students.RemoveAll(x => x.Id == _studentId);            
            else
                AssignIdToNewStudent(students);

            AddNewUserToList(students);
          
            _fileHelper.SerializeToFile(students);

            Close();
        }

        private void AddNewUserToList(List<Student> students)
        {
            var student = new Student
            {
                Id = _studentId,
                FirstName = tbName.Text,
                LastName = tbLastName.Text,
                Comments = rtbComments.Text,
                ForeignLang = tbForeign.Text,
                Math = tbMath.Text,
                Physics = tbPhysics.Text,
                PolishLang = tbPolish.Text,
                Technology = tbTechnology.Text,
                Extra = cbExtra.Checked,
                GroupId = (cbGroupId.SelectedItem as Group).Id,
            };

            students.Add(student);
        }

        private void AssignIdToNewStudent(List<Student> students)
        {
            var studentWithHighestId = students
           .OrderByDescending(x => x.Id).FirstOrDefault();

            _studentId = studentWithHighestId == null ?
                1 : studentWithHighestId.Id + 1;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }


    }
}
