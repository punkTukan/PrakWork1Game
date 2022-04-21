using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrakWork1Game
{
    public partial class Form2 : Form
    {
        public const int gridSize = 4;

        public bool closedByAnoterForm = false;

        Bot500iq botty;

        string currentPlayer = "Player one";

        string currentPlayerHasToScore = "";
        
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            Random random = new Random();
            dataGridView1.Rows.Add(gridSize-1);
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    dataGridView1.Rows[i].Cells[j].Value = random.Next(4)+1;
                    dataGridView1.Rows[i].Height = 80;
                }
            }
            updateScore();

            if (checkForGameEnd())
            {
                gameEnd();
            }

            if (Options.playModeAgainstPc)
            {
                List<List<int>> field = new List<List<int>>();

                foreach (DataGridViewRow dr in dataGridView1.Rows)
                {
                    List<int> item = new List<int>();
                    foreach (DataGridViewCell dc in dr.Cells)
                    {
                        item.Add(Convert.ToInt32(dc.Value));
                    }
                    field.Add(item);
                }
                bool maximizing;
                if (Options.firstPlyerToScoreLargerThanTwo)
                {
                    maximizing = true;
                }
                else
                {
                    maximizing=false;
                }

                botty = new Bot500iq(field, maximizing);
            }
            
            if (Options.firstPlyerToScoreLargerThanTwo)
            {
                currentPlayerHasToScore = "Has to score >= 2";
            }
            else
            {
                currentPlayerHasToScore = "Has to score < 2";
            }

            label4.Text = currentPlayerHasToScore;
            
            if (!Options.firstPlayerMovesFirst)
            {
                changeCurrentPlayer();
                label3.Text = currentPlayer;
                if (Options.playModeAgainstPc)
                {
                    this.Show();
                    pcMakesATurn();
                }
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count > 1)
            {
                dataGridCleanupAfterSelect();
                updateScore();
                changeCurrentPlayer();
                label3.Text = currentPlayer;
                changeHasToScore();
                label4.Text = currentPlayerHasToScore;
                this.Refresh();
                if (!checkForGameEnd())
                {
                    if (Options.playModeAgainstPc)
                    {
                        System.Threading.Thread.Sleep(1500);
                        pcMakesATurn();
                    }
                }
                else
                {
                    gameEnd();
                }
            }
        }

        private void pcMakesATurn()
        {
            botMakesAMove();
            updateScore();
            changeCurrentPlayer();
            label3.Text = currentPlayer;
            changeHasToScore();
            label4.Text = currentPlayerHasToScore;
            if (checkForGameEnd())
            {
                gameEnd();
            }
        }

        protected override void OnClick(EventArgs e)
        {
            dataGridView1.MultiSelect = false;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //dataGridView1.Rows[2].Cells[2].Selected = true;
            int row = dataGridView1.CurrentCell.RowIndex;
            int column = dataGridView1.CurrentCell.ColumnIndex;
            string msg = string.Format("Row: {0}, Column: {1}", row, column);

            List<List<int>> items = new List<List<int>>();
            foreach (DataGridViewRow dr in dataGridView1.Rows)
            {
                List<int> item = new List<int>();
                foreach (DataGridViewCell dc in dr.Cells)
                {
                    item.Add(Convert.ToInt32(dc.Value));
                }

                items.Add(item);
            }


            List<Tuple<int,int>> itemsforcheck = new List<Tuple<int,int>>();
            itemsforcheck.Add(Tuple.Create(row, column));
            List<Tuple<int, int>> selectCells = checkForNearSimillarNumbers(items, itemsforcheck, itemsforcheck.ElementAt(0), items.ElementAt(row).ElementAt(column));
            dataGridView1.MultiSelect = true;
            foreach (Tuple<int, int> item in selectCells)
            {
                dataGridView1.Rows[item.Item1].Cells[item.Item2].Selected = true;
            }
            
            /*MessageBox.Show(msg);*/

        }

        public static List<Tuple<int,int>> checkForNearSimillarNumbers(List<List<int>> board, List<Tuple<int, int>> isSameNumber, Tuple<int, int> checkForSameNumber, int sameNumber) 
        {
            int row = checkForSameNumber.Item1;
            int column = checkForSameNumber.Item2;

            if (row > 0)
            {
                int newRow = row - 1;
                if (board.ElementAt(newRow).ElementAt(column) == sameNumber && !isSameNumber.Contains(Tuple.Create(newRow, column)) )
                {
                    isSameNumber.Add(Tuple.Create(newRow, column));
                    isSameNumber = checkForNearSimillarNumbers(board, isSameNumber, Tuple.Create(newRow, column), sameNumber);
                }
            }
            if (row+1 < gridSize)
            {
                int newRow = row + 1;
                if (board.ElementAt(newRow).ElementAt(column) == sameNumber && !isSameNumber.Contains(Tuple.Create(newRow, column)))
                {
                    isSameNumber.Add(Tuple.Create(newRow, column));
                    isSameNumber = checkForNearSimillarNumbers(board, isSameNumber, Tuple.Create(newRow, column), sameNumber);
                }
            }

            if (column > 0)
            {
                int newColumn = column - 1;
                if (board.ElementAt(row).ElementAt(newColumn) == sameNumber && !isSameNumber.Contains(Tuple.Create(row, newColumn)))
                {
                    isSameNumber.Add(Tuple.Create(row, newColumn));
                    isSameNumber = checkForNearSimillarNumbers(board, isSameNumber, Tuple.Create(row, newColumn), sameNumber);
                }
            }
            if (column + 1 < gridSize)
            {
                int newColumn = column + 1;
                if (board.ElementAt(row).ElementAt(newColumn) == sameNumber && !isSameNumber.Contains(Tuple.Create(row, newColumn)))
                {
                    isSameNumber.Add(Tuple.Create(row, newColumn));
                    isSameNumber = checkForNearSimillarNumbers(board, isSameNumber, Tuple.Create(row, newColumn), sameNumber);
                }
            }

            return isSameNumber;
        }

        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            dataGridView1.MultiSelect = false; 
        }

        private void gameEnd()
        {
            dataGridView1.Enabled = false;
            button1.Enabled = false;
            
            string message = "GAME OVER\n";
            GameResultForm grF = new GameResultForm();
            double score = Double.Parse(label1.Text);
            string firstPlayer = "", secondPlayer ="";
            if (Options.firstPlayerMovesFirst)
            {
                firstPlayer = "Player 1";
                if (Options.playModeAgainstPc)
                {
                    secondPlayer = "PC";
                }
                else {
                    secondPlayer = "Player 2";
                }
            }
            else {
                secondPlayer = "Player 1";
                if (Options.playModeAgainstPc)
                {
                    firstPlayer = "PC";
                }
                else
                {
                    firstPlayer = "Player 2";
                }
            }

            if (score >= 2)
            {
                if (Options.firstPlyerToScoreLargerThanTwo)
                {
                    message += firstPlayer + " won!\n";
                }
                else
                {
                    message += secondPlayer + " won!\n";

                }
            }
            else
            {
                if (Options.firstPlyerToScoreLargerThanTwo)
                {
                    message += secondPlayer + " won!\n";
                }
                else
                {
                    message += firstPlayer + " won!\n";

                }
            }
            grF.label2.Text = message;
            grF.label2.Left = (grF.Width - grF.label2.Width) / 2;
            grF.Show();
        }

        private void botMakesAMove()
        {
            List<List<int>> field = new List<List<int>>();

            foreach (DataGridViewRow dr in dataGridView1.Rows)
            {
                List<int> item = new List<int>();
                foreach (DataGridViewCell dc in dr.Cells)
                {
                    item.Add(Convert.ToInt32(dc.Value));
                }
                field.Add(item);
            }
            botty.understandPlayerTurn(field);
            field = botty.decideOnATurn();



            for (int i = 0; i < field.Count; i++)
            {
                for (int j = 0; j < field[i].Count; j++)
                {
                    if (field[i][j] != 0)
                    {
                        dataGridView1.Rows[i].Cells[j].Value = field[i][j];
                    }
                    else
                    {
                        dataGridView1.Rows[i].Cells[j].Value = null;
                    }
                }
            }
        }

        private void dataGridCleanupAfterSelect ()
        {
            foreach (DataGridViewCell item in dataGridView1.SelectedCells)
            {
                item.Value = null;
            }
            moveItemsDown();
            moveItemsRight();
            dataGridView1.ClearSelection();
        }

        private void moveItemsDown()
        {
            for (int i = 0; i < dataGridView1.Rows[0].Cells.Count; i++)
            {
                for (int j = dataGridView1.Rows.Count-1; j > 0; j--)
                {
                    if (dataGridView1.Rows[j].Cells[i].Value == null)
                    {
                        int currentHeight = j;
                        while (currentHeight > 0)
                        {
                            if  (dataGridView1.Rows[currentHeight-1].Cells[i].Value != null)
                            {
                                dataGridView1.Rows[j].Cells[i].Value = dataGridView1.Rows[currentHeight - 1].Cells[i].Value;
                                dataGridView1.Rows[currentHeight - 1].Cells[i].Value = null;
                                break;
                            }
                            currentHeight--;
                        }
                    }
                }
            }
        }

        private void moveItemsRight()
        {
            for (int i = dataGridView1.Rows[0].Cells.Count-1; i > 0; i--)
            {
                if (dataGridView1.Rows[gridSize - 1].Cells[i].Value == null)
                {
                    int currentColumn = i;
                    while (currentColumn > 0)
                    {
                        if (dataGridView1.Rows[gridSize - 1].Cells[currentColumn - 1].Value != null)
                        {
                            moveColumn(currentColumn-1, i);
                            break;
                        }
                        currentColumn--;
                    }
                }
            }
        }

        private void moveColumn(int columnMoveFromIndex, int columnMoveToIndex)
        {
            int i = gridSize - 1;
            while (i >= 0 && dataGridView1.Rows[i].Cells[columnMoveFromIndex] != null)
            {
                dataGridView1.Rows[i].Cells[columnMoveToIndex].Value = dataGridView1.Rows[i].Cells[columnMoveFromIndex].Value;
                dataGridView1.Rows[i].Cells[columnMoveFromIndex].Value = null;
                i--;
            }
        }

        private bool checkForGameEnd()
        {
            for (int i = gridSize-1; i >= 1; i--)
            {
                for (int j = gridSize - 1; j >= 0; j--)
                {
                    var currentValue = dataGridView1.Rows[i].Cells[j].Value;
                    var upCell = dataGridView1.Rows[i - 1].Cells[j].Value ?? 0;
                    object leftCell = 0;
                    if (j > 0)
                    {
                        leftCell = dataGridView1.Rows[i].Cells[j - 1].Value ?? 0;
                    }
                    else
                    {
                        leftCell = 0;
                    }
                    if (currentValue != null && (leftCell.Equals(currentValue) || upCell.Equals(currentValue)))
                    {
                        return false;
                    }
                }
            }
            return true; 
        }

        private void updateScore()
        {
            int sum = 0, count = 0;
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    if (dataGridView1.Rows[i].Cells[j].Value != null)
                    {
                        sum += (int) dataGridView1.Rows[i].Cells[j].Value;
                        count++;
                    }
                }
            }
            double result = Math.Round((double) sum / count, 2);
            label1.Text = result.ToString();
            return;
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!closedByAnoterForm) { 
                Application.OpenForms["Form1"].Show();
            }
        }

        private void changeCurrentPlayer()
        {
            if (currentPlayer == "Player one" && Options.playModeAgainstPc)
            {
                currentPlayer = "PC";
            }
            else if (currentPlayer == "Player one" && !Options.playModeAgainstPc)
            {
                currentPlayer = "Player two";
            }
            else if (currentPlayer == "Player two")
            {
                currentPlayer = "Player one";
            }
            else if (currentPlayer == "PC")
            {
                currentPlayer = "Player one";
            }
        }

        private void changeHasToScore()
        {
            if (currentPlayerHasToScore == "Has to score >= 2")
            {
                currentPlayerHasToScore = "Has to score < 2";
            }
            else if (currentPlayerHasToScore == "Has to score < 2")
            {
                currentPlayerHasToScore = "Has to score >= 2";
            }
        }

    }
}
