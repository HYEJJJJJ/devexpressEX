using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.ClipboardSource.SpreadsheetML;
using System.Data.OleDb;
using System.Text.RegularExpressions;

namespace People_gridview
{
    public partial class InfoForm : DevExpress.XtraEditors.XtraForm
    {
        //DataTable 객체 생성
        DataTable tbl = new DataTable();

        //텍스트파일 경로
        string txtFilePath = "C:\\Users\\user\\Desktop\\data.txt";


        public InfoForm()
        {
            InitializeComponent();

            //Column 이름 지정하는 List
            //List<string> colName = new List<string> { "Name", "Age", "Gender", "Email", "PhoneNumber" };
            //for (int i = 0; i < colName.Count(); i++)
            //{
            //    tbl.Columns.Add(colName[i], typeof(string));
            //}

            ColumnData column = new ColumnData();

            //Column 이름 지정하는 List
            foreach (System.Reflection.PropertyInfo property in column.GetType().GetProperties())
            {
                tbl.Columns.Add(property.Name, typeof(string));
            }

            gridControl1.DataSource = tbl;

            // ColumnChanged 이벤트 핸들러 등록
            tbl.ColumnChanged += tbl_ColumnChanged;

            //commit과 같은 기능
            tbl.AcceptChanges();


            gridView1.OptionsBehavior.Editable = false; // 모든 셀에디터 비활성화
        }

        //폼이 로드되면
        private void InfoForm_Load(object sender, EventArgs e)
        {
            ColumnData column = new ColumnData();

            //콤보박스 아이템 추가
            foreach (System.Reflection.PropertyInfo property in column.GetType().GetProperties())
            {
                cbSearch.Properties.Items.Add(property.Name);
            }

            cbSearch.SelectedIndex = -1;
            cbSearch.Text = "선택";
            
            btnClear.Enabled = btnDelete.Enabled = btnEdit.Enabled = btnExport.Enabled = btnSearch.Enabled = false;
            
        }



        //조회버튼 클릭시
        private void btnLoad_Click(object sender, EventArgs e)
        {
            tbl.Clear();

            //textData 메서드를 호출하여 데이터 가져오기
            List<ColumnData> dataList = readTextData(txtFilePath);

            //위에서 가져온 데이터를 추가하기
            foreach (ColumnData column in dataList)
            {
                DataRow newRow = tbl.NewRow(); //데이터의 행 생성

                foreach (System.Reflection.PropertyInfo property in column.GetType().GetProperties())
                {
                    newRow[property.Name] = property.GetValue(column);
                }

                tbl.Rows.Add(newRow);
            }

            tbl.AcceptChanges();
            btnClear.Enabled = btnDelete.Enabled = btnEdit.Enabled = btnExport.Enabled = btnSearch.Enabled = true;
            gridView1.OptionsBehavior.Editable = false; // 모든 셀에디터 비활성화


        }

        //txt파일 행을 읽어오는 메서드
        public List<ColumnData> readTextData(string path)
        {
            //ColumnData 클래스를 제네릭으로 하는 List 객체 생성
            List<ColumnData> dataList = new List<ColumnData>();

            //using문을 사용하면 Resource를 사용한 후에 자동으로 정리 및 해제되므로 메모리 낭비를 방지
            //파일을 읽거나 쓰는 작업을 위해 FileStream객체를 사용하는 경우
            //using문을 사용하여 fileStream을 정의하면
            //이를 사용한 후 using문에서 벗어나면 자동으로 닫히므로 별도로 파일을 닫아주는 코드를 작성할 필요가 없다.

            //StreamReader : 스트림에서 문자열을 읽음
            using (StreamReader sr = new StreamReader(path))
            {
                //읽을 수 있는 문자가 있다면, 한줄씩 읽어와서 dataList에 추가
                while (sr.Peek() > 0)
                {
                    string line = sr.ReadLine();

                    string[] data = line.Split(' ');
                    dataList.Add(new ColumnData(data[0], data[1], data[2], data[3], data[4]));
                }
            }

            //dataList로 반환
            return dataList;

        }


        //추가버튼 클릭시
        private void btnInsert_Click(object sender, EventArgs e)
        {
            gridView1.OptionsBehavior.Editable = true; // 모든 셀에디터 활성화

            //새로운 행 생성
            DataRow newRow = tbl.NewRow();
            tbl.Rows.Add(newRow);

            tbl.AcceptChanges();

        }

        //수정버튼 클릭시
        private void btnEdit_Click(object sender, EventArgs e)
        {
            gridView1.OptionsBehavior.Editable = true; // 모든 셀에디터 활성화
            tbl.AcceptChanges();
        
        }

        //삭제버튼 클릭시 
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                //GetSelectedRows() 메서드는 선택된 행의 인덱스를 반환하는 배열을 리턴
                foreach (int i in gridView1.GetSelectedRows())
                {
                    //현재 반복 중인 인덱스 i에 해당하는 행의 데이터를 가져옴 = 선택된 행
                    DataRow row = gridView1.GetDataRow(i);
                    tbl.Rows.Remove(row);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            //DataTable의 행이 없다면 = 데이터가 존재하지 않는다면
            if (tbl.Rows.Count == 0)
            {
                MessageBox.Show("삭제할 행이 없습니다. txt 파일을 조회하거나 행을 생성하세요.");
                btnClear.Enabled = btnDelete.Enabled = btnEdit.Enabled = btnExport.Enabled = btnSearch.Enabled = false;
            }
        }

        //일괄 삭제 버튼 클릭시
        private void btnClear_Click(object sender, EventArgs e)
        {
            tbl.Clear();
            tbl.AcceptChanges();
            btnClear.Enabled = btnDelete.Enabled = btnEdit.Enabled = btnExport.Enabled = btnSearch.Enabled = false;
        }


        //찾기버튼 클릭시
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = teKeyword.Text;

            //콤보박스의 값이 '선택'이라면
            if(cbSearch.EditValue.ToString() == "선택")
            {
                MessageBox.Show("검색할 조건을 선택하세요");
            }

            else
            {
                 string searchBy = cbSearch.EditValue.ToString();

                 // 적합한 필터를 선택
                 string filter = $"{searchBy} = '{keyword}'";

                 // 선택된 필터를 적용
                 tbl.DefaultView.RowFilter = filter;

                 // 필터링된 결과를 DataTable로 변환
                 DataTable resultDt = tbl.DefaultView.ToTable();

                 // 필터링된 결과가 없을 때 처리
                 if (resultDt.Rows.Count == 0)
                 {
                    MessageBox.Show("일치하는 결과가 없습니다.");

                    // 필터 해제하고 모든 데이터를 다시 보이도록 설정
                    tbl.DefaultView.RowFilter = "";

                    // 필터 해제된 결과를 DataTable로 변환
                    DataTable originalDt = tbl.DefaultView.ToTable();
                 }
            }
        }

        //추출버튼 클릭시
        private void btnExport_Click(object sender, EventArgs e)
        {
            WriteDataToFile(tbl, "C:\\Users\\user\\Desktop\\newData.txt");
        }

        //DataTable 행을 읽어오는 메서드
        public static void WriteDataToFile(DataTable submittedDataTable, string submittedFilePath)
        {
            int i = 0;
            //StreamWriter = 구현하려는 txt파일 스트림에 문자열 작성
            using (StreamWriter sw = new StreamWriter(submittedFilePath, false))
            {
               //반복문을 이용하여 DataTable의 Row을 읽어와서 작성
                foreach (DataRow row in submittedDataTable.Rows)
                {
                    object[] array = row.ItemArray;

                    for (i = 0; i < array.Length - 1; i++)
                    {
                        sw.Write(array[i].ToString() + " ");
                    }
                    sw.Write(array[i].ToString());
                    sw.WriteLine();

                }

                MessageBox.Show("추출에 성공했습니다 ! ");
            }
        }

        //열값 변경시 이벤트 핸들러
        private void tbl_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (!Regex.IsMatch(e.Row[e.Column].ToString(), GetPattern(e.Column.ColumnName)))
            {
                MessageBox.Show("변경한 데이터가 유효하지 않습니다. 원래 값으로 복구합니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Row.RejectChanges();
            }           
        }

        //유효성 검사하는 메서드
        private string GetPattern(string colName)
        {
            string returnValue = "";
            if (colName == "Name") returnValue = @"^[가-힣]*$";
            else if (colName == "Email") returnValue = @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}";
            else if (colName == "Age") returnValue = @"^(?:[1-9][0-9]?|1[01][0-9]|120)$";
            else if (colName == "Gender") returnValue =  @"^(남자|여자)$";
            else if (colName == "PhoneNumber") returnValue = @"^010-\d{4}-\d{4}$"; // 010-nnnn-nnnn

            return returnValue;
        }

    }
}
