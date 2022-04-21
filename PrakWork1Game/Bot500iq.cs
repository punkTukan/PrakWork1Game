using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrakWork1Game
{
    internal class Bot500iq
    {
        const int gridSize = Form2.gridSize;

        List<List<int>> startingField;

        Tree mainTree;

        bool playingForMaximizing;

        Node currentStateOfGame;

        public Bot500iq(List<List<int>> field, bool playingForMaximizing)
        {
            this.startingField = field;
            this.playingForMaximizing = playingForMaximizing;
            mainTree = new Tree();
            mainTree = makeATree(field);
            alphaBeta(mainTree.root, -10, 10, playingForMaximizing);
        }

        public void understandPlayerTurn(List<List<int>> field)
        {
            foreach (var child in currentStateOfGame.childNodes)
            {
                if (fieldsMatch(child.stateOfGame, field))
                {
                    currentStateOfGame = child;
                }
            }

        }

        public List<List<int>> decideOnATurn()
        {
            if (!Options.firstPlyerToScoreLargerThanTwo)
            {
                if (!currentStateOfGame.childNodes.All(x => x.result != 1))
                {
                    currentStateOfGame.childNodes.RemoveAll(x => x.result != 1);
                }
            }
            else if (Options.firstPlyerToScoreLargerThanTwo)
            {
                if (!currentStateOfGame.childNodes.All(x => x.result == 1))
                {
                    currentStateOfGame.childNodes.RemoveAll(x => x.result == 1);
                }
            }
            /*Random random = new Random();
            int select = random.Next(currentStateOfGame.childNodes.Count);*/
            currentStateOfGame = currentStateOfGame.childNodes.First();
            return currentStateOfGame.stateOfGame;
        }

        Tree makeATree(List<List<int>> field)
        {

            mainTree.root = addNodes(null, field);
            currentStateOfGame = mainTree.root;
            return mainTree;
        }

        short alphaBeta (Node node, int alpha, int beta, bool maximizingPlayer )
        {
            if (node.isEnd)
            {
                return node.result;
            }
            if (maximizingPlayer)
            {
                short value = -10;
                foreach (var child in node.childNodes)
                {
                    value = Math.Max(value, alphaBeta(child, alpha, beta, false));
                    alpha = Math.Max(alpha, value);
                    if (value > beta)
                    {
                        break;
                    }
                }
                node.result = value;
                return value;
            }
            else
            {
                short value = 10;
                foreach (var child in node.childNodes)
                {
                    value = Math.Min(value, alphaBeta(child, alpha, beta, false));
                    beta = Math.Min(beta, value);
                    if (value < alpha)
                    {
                        break;
                    }
                }
                node.result = value;
                return value;
            }
  
        }

        Node addNodes(Node parent, List<List<int>> field)
        {
            Node nNode = new Node(parent, field);
            bool gameEnded = checkForGameEnd(field);
            if (!gameEnded)
            {
                nNode.isEnd = false;
                List<List<Tuple<int, int>>> possibleTurns = findAllTurns(field);
                foreach (var turn in possibleTurns)
                {
                    List<List<int>> fieldN = new List<List<int>>(field.Select(x => x.ToList()));
                    fieldN = makeATurn(fieldN, turn);
                    Node nnNode = new Node(nNode);
                    nnNode = addNodes(nnNode, fieldN);
                    nNode.childNodes.Add(nnNode);
                }
            }
            else
            {
                nNode.isEnd = true;
                double score = calculateResult(field);
                if (score >= 2)
                {
                    nNode.result = 1;
                }
                else
                {
                    nNode.result = -1;
                }
            }

            return nNode;
        }

        List<List<Tuple<int, int>>> findAllTurns(List<List<int>> field)
        {
            List<Tuple<int, int>> possibleTurns = new List<Tuple<int, int>>();
            List<List<Tuple<int, int>>> possibleTurnsWithTurnNumbers = new List<List<Tuple<int, int>>>();
            int turnNumber = 0;
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    Tuple<int, int> turn = new Tuple<int, int>(i, j);
                    if (field.ElementAt(i).ElementAt(j) != 0 && !possibleTurns.Contains(turn))
                    {
                        List<Tuple<int, int>> itemsforcheck = new List<Tuple<int, int>>();
                        itemsforcheck.Add(Tuple.Create(i, j));
                        List<Tuple<int, int>> newPossibleTurns = Form2.checkForNearSimillarNumbers(field, itemsforcheck, itemsforcheck.First(), field.ElementAt(i).ElementAt(j));
                        if (newPossibleTurns.Count > 1)
                        {
                            possibleTurnsWithTurnNumbers.Add(new List<Tuple<int, int>>());
                            foreach (Tuple<int, int> item in newPossibleTurns)
                            {
                                possibleTurns.Add(item);
                                possibleTurnsWithTurnNumbers[turnNumber].Add(item);
                                /*Tuple<int, int, int> turnWithNumber = new Tuple<int, int, int>(item.Item1, item.Item2, turnNumber);
                                possibleTurnsWithTurnNumbers.Add(turnWithNumber);*/
                            }
                            turnNumber++;
                        }
                    }
                }
            }

            return possibleTurnsWithTurnNumbers;
        }

        public static double calculateResult(List<List<int>> field)
        {
            int sum = 0, count = 0;
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    if (field.ElementAt(i).ElementAt(j) != 0)
                    {
                        sum += field.ElementAt(i).ElementAt(j);
                        count++;
                    }
                }
            }
            double result = Math.Round((double)sum / count, 2);
            return result;
        }

        bool checkForGameEnd(List<List<int>> field)
        {
            int currentValue = 0, upCell = 0, leftCell = 0;
            for (int i = gridSize - 1; i >= 1; i--)
            {
                for (int j = gridSize - 1; j >= 0; j--)
                {
                    currentValue = field.ElementAt(i).ElementAt(j);
                    upCell = field.ElementAt(i - 1).ElementAt(j);
                    leftCell = 0;
                    if (j > 0)
                    {
                        leftCell = field.ElementAt(i).ElementAt(j - 1);
                    }
                    else
                    {
                        leftCell = -1;
                    }
                    if (currentValue != 0 && (leftCell == currentValue || upCell == currentValue))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        List<List<int>> makeATurn(List<List<int>> startField, List<Tuple<int, int>> turn)
        {
            foreach (var cell in turn)
            {
                int row = cell.Item1;
                int column = cell.Item2;
                startField[row][column] = 0;
            }
            startField = moveItemsDown(startField);
            startField = moveItemsRight(startField);
            return startField;
        }

        private List<List<int>> moveItemsDown(List<List<int>> field)
        {
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = gridSize - 1; j > 0; j--)
                {
                    if (field[j][i] == 0)
                    {
                        int currentHeight = j;
                        while (currentHeight > 0)
                        {
                            if (field[currentHeight-1][i] != 0)
                            {
                                field[j][i] = field[currentHeight-1][i];
                                field[currentHeight - 1][i] = 0;
                                break;
                            }
                            currentHeight--;
                        }
                    }
                }
            }
            return field;
        }

        private List<List<int>> moveItemsRight(List<List<int>> field)
        {
            for (int i = gridSize - 1; i > 0; i--)
            {
                if (field[gridSize-1][i] == 0)
                {
                    int currentColumn = i;
                    while (currentColumn > 0)
                    {
                        if (field[gridSize-1][currentColumn-1] != 0)
                        {
                            field = moveColumn(field, currentColumn - 1, i);
                            break;
                        }
                        currentColumn--;
                    }
                }
            }
            return field;
        }

        private List<List<int>> moveColumn(List<List<int>> field, int columnMoveFromIndex, int columnMoveToIndex)
        {
            int i = gridSize - 1;
            while (i >= 0 && field[i][columnMoveFromIndex] != 0)
            {
                field[i][columnMoveToIndex] = field[i][columnMoveFromIndex];
                field[i][columnMoveFromIndex] = 0; 
                i--;
            }
            return field;
        }

        static public String printField(List<List<int>> field)
        {
            string finStr = "";
            foreach (List<int> row in field)
            {
                foreach (int i in row)
                {
                    finStr += i + "  ";
                }
                finStr += "\n";
            }
            return finStr;
        }

        bool fieldsMatch (List<List<int>> field1, List<List<int>> field2)
        {
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    if (field1[i][j] != field2[i][j])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

    }


    class Tree
    {
        public Node root { get; set; }

        public void addNode(Node parent, Node child)
        {
            parent.childNodes.Add(child);
        }

        public string printTreeResults (Node currentNode)
        {
            string str = "";
            if (!currentNode.isEnd)
            {
                foreach (Node child in currentNode.childNodes)
                {
                    str += printTreeResults(child);
                }
            }
            else
            {
                str += "---------------------\n";
                str += Bot500iq.printField(currentNode.stateOfGame);
                str += "result is - " + Bot500iq.calculateResult(currentNode.stateOfGame) + "\n";
                str += "that means - " + currentNode.result + "\n";
            }

            return str;
        }

    }

    class Node
    {   
        public Node parent;
        public List<Node> childNodes = new List<Node>();
        public List<List<int>> stateOfGame;
        public bool isEnd = false;
        public short result = 0; // -1 - won <2 , 1 - won >=2, 0 - undecided

        public Node (Node parent, List<List<int>> stateOfGame)
        {
            this.parent = parent;
            this.stateOfGame = stateOfGame;
        }

        public Node (Node n)
        {
            this.parent = n.parent;
            this.childNodes = n.childNodes;
            this.stateOfGame = n.stateOfGame;
            this.isEnd = n.isEnd;
            this.result = n.result;
        }

        Node (Node parent, List<List<int>> stateOfGame, bool isEnd, short result)
        {
            this.parent = parent;
            this.stateOfGame = stateOfGame;
            this.isEnd = isEnd;
            this.result = result; 
        }
    }


}


/*
 * 
 * tests
string outp = "";
int turnNum = 1;
*//*foreach (var turn in possibleTurns)
{
    foreach (var turn2 in turn)
    {
        outp += "y = " + turn2.Item1 + " x = " + turn2.Item2 + " n = " + turnNum + "\n";
    }
    turnNum++;
}
outp += "Score = " + score + "\n";
if (ch)
{
    outp += "Game is over \n";
}
else
{
    outp += "Game is NOT over \n";
}*//*
foreach (var turn in possibleTurns)
{
    List<List<int>> fieldf = new List<List<int>>(field.Select(x => x.ToList()));
    outp += printField(makeATurn(fieldf, turn));
    outp += "\n\n";
}


MessageBox.Show(outp);*/