using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace _010_Omok
{
  public partial class Form1 : Form
  {
    Graphics g;
    int 눈Size = 40;  // 눈금의 크기
    int 돌Size = 38; // 돌 사이즈
    int 화점Size = 8;
    int margin = 20;  // 패널 주변과 바둑판의 간격
    Pen pen;
    Brush wBrush, bBrush;
    enum STONE { none, black, white};
    STONE[,] 바둑판 = new STONE[19, 19];
    private bool flag = false;
    private bool imageFlag = true;  // true이면 이미지로 그림
    List<Bokki> list = new List<Bokki>();

    public Form1()
    {
      InitializeComponent();

      this.Text = "Omok 0.5 by bikang";
      this.BackColor = Color.Orange;
      pen = new Pen(Color.Black);
      wBrush = new SolidBrush(Color.White);
      bBrush = new SolidBrush(Color.Black);

      // 중요! 
      ClientSize = new Size(2 * margin + 18 * 눈Size,
        2 * margin + 18 * 눈Size + menuStrip1.Height);
    }

    // OnPaint() 함수
    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);

      Draw바둑판();
      Draw돌들();
    }

    // 자료구조에서 돌들을 읽어서 다시 그려줌
    private void Draw돌들()
    {
      for (int i = 0; i < 19; i++)
        for (int j = 0; j < 19; j++)
        {
          if (바둑판[i, j] != STONE.none)
            DrawAStone(i, j);
        }
    }

    private void Draw바둑판()
    {
      g = panel1.CreateGraphics();

      // 가로줄
      for(int i=0; i<19; i++)
      {
        g.DrawLine(pen, new Point(margin, margin + 눈Size * i),
          new Point(margin + 18 * 눈Size, margin + 눈Size * i));
      }

      // 세로줄
      for(int i=0; i<19; i++)
      {
        g.DrawLine(pen,
          new Point(margin + 눈Size * i, margin), 
          new Point(margin + 눈Size * i, margin + 눈Size * 18));
      }

      // 화점 
      // Rectangle(int x, int y, int w, int h)
      // g.FillEllipse(Brush, Rectangle)
      for(int i=3; i<=15; i+=6)
        for(int j=3; j<=15; j+=6)
        {
          g.FillEllipse(bBrush,
            new Rectangle(margin + i * 눈Size - 화점Size/2, margin + j * 눈Size - 화점Size/2, 화점Size, 화점Size));
        }
    }

    // panel1에서 마우스 다운 이벤트 처리 함수
    private void panel1_MouseDown(object sender, MouseEventArgs e)
    {
      // 조금 더 정교할 필요가 있음
      int x = (e.X - margin + 눈Size / 2) / 눈Size;
      int y = (e.Y - margin + 눈Size / 2) / 눈Size;

      // (x,y)에 돌을 그리자
      if (바둑판[x, y] != STONE.none)
        return;

      DrawAStone(x, y);
      SaveList(x, y);
      
      CheckOmok(x, y);
    }

    // 테스트를 위해 List<Bokki> list를 출력
    private void PrintList()
    {
      string s = "";
      foreach(Bokki su in list)
      {
        s += su.X + "," + su.Y + ":" + su.Stone + "\n";
      }
      MessageBox.Show(s);
    }

    // 복기를 위해 현재 위치와 돌 색깔을 리스트에 저장
    private void SaveList(int x, int y)
    {
      if (바둑판[x, y] == STONE.black)
        list.Add(new Bokki(x, y, 'b'));
      else if (바둑판[x, y] == STONE.white)
        list.Add(new Bokki(x, y, 'w'));
    }

    // 바둑판[x, y]에 돌을 하나 그림
    private void DrawAStone(int x, int y)
    {
      // 바둑판[x, y]에 돌을 그리기 위한 사각형
      Rectangle r = new Rectangle(
        margin + 눈Size * x - 돌Size / 2,
        margin + 눈Size * y - 돌Size / 2,
        돌Size, 돌Size);

      // 이미지 또는 FillEllipse()
      if (flag == true)
      {
        if (imageFlag == false)
          g.FillEllipse(wBrush, r);
        else
        {
          Bitmap bmp = new Bitmap("../../images/white.png");
          g.DrawImage(bmp, r);
        }
        바둑판[x, y] = STONE.white;
        flag = false;
      }
      else
      {
        if (imageFlag == false)
          g.FillEllipse(bBrush, r);
        else
        {
          Bitmap bmp = new Bitmap("../../images/black.png");
          g.DrawImage(bmp, r);
        }
        바둑판[x, y] = STONE.black;
        flag = true;
      }
    }

    // 함수중복(복기할 때 사용) : 바둑판[x, y]에 Stone색 돌을 하나 그림
    private void DrawAStone(int x, int y, STONE stone)
    {
      // 바둑판[x, y]에 돌을 그리기 위한 사각형
      Rectangle r = new Rectangle(
        margin + 눈Size * x - 돌Size / 2,
        margin + 눈Size * y - 돌Size / 2,
        돌Size, 돌Size);

      // 이미지 또는 FillEllipse()
      if (stone == STONE.white)
      {
        if (imageFlag == false)
          g.FillEllipse(wBrush, r);
        else
        {
          Bitmap bmp = new Bitmap("../../images/white.png");
          g.DrawImage(bmp, r);
        }
        바둑판[x, y] = STONE.white;
        flag = false;
      }
      else
      {
        if (imageFlag == false)
          g.FillEllipse(bBrush, r);
        else
        {
          Bitmap bmp = new Bitmap("../../images/black.png");
          g.DrawImage(bmp, r);
        }
        바둑판[x, y] = STONE.black;
        flag = true;
      }
    }

    // 지금 놓인 돌에 의해 오목이 만들어졌는지를 체크하는 함수
    private void CheckOmok(int x, int y)
    {
      int cnt = 1;

      // [x,y]의 오른쪽으로 체크
      for (int i=1; i<5; i++)
      {
        if (x + i <= 18 && 바둑판[x + i, y] == 바둑판[x, y])
          cnt++;
        else
          break;
      }

      // [x,y]의 왼쪽으로 체크
      for(int i=1; i<5; i++)
      {
        if (x - i >= 0 && 바둑판[x - i, y] == 바둑판[x, y])
          cnt++;
        else
          break;
      }

      if (cnt >= 5)
        MessageBox.Show(바둑판[x, y] + " Wins!");
    }

    private void Form1_Move(object sender, EventArgs e)
    {
      Draw바둑판();
      Draw돌들();
    }

    private void 그리기ToolStripMenuItem_Click(object sender, EventArgs e)
    {
      imageFlag = false;
    }

    private void 이미지ToolStripMenuItem_Click(object sender, EventArgs e)
    {
      imageFlag = true;
    }

    // 다시시작 메뉴 처리함수
    private void 다시시작ToolStripMenuItem_Click(object sender, EventArgs e)
    {
      // 자료구조 초기화
      for (int i = 0; i < 19; i++)
        for (int j = 0; j < 19; j++)
          바둑판[i, j] = STONE.none;

      // flag 초기화: 흑돌부터
      flag = false;

      // 바둑판 다시 그리기
      panel1.Refresh();
      Draw바둑판();
    }

    Timer timer; 
    // 복기는 0.5초에 하나씩 그려준다
    private void 복기ToolStripMenuItem_Click(object sender, EventArgs e)
    {
      PrintList();
      panel1.Refresh();
      Draw바둑판();

      timer = new Timer();
      timer.Interval = 500;
      timer.Start();
      timer.Tick += Timer_Tick;

    }

    int ListIndex = 0;

    private void Timer_Tick(object sender, EventArgs e)
    {
      DrawAStone(list[ListIndex].X, list[ListIndex].Y, 
        list[ListIndex].Stone == 'b' ? STONE.black : STONE.white);
      ListIndex++;
      if (ListIndex >= list.Count)
      {
        timer.Stop();
        ListIndex = 0;
      }
        
      //foreach (Bokki su in list)
      //{
      //  DrawAStone(su.X, su.Y, (su.Stone == 'b' ? STONE.black : STONE.white));
      //}      
    }

    // 기보 저장하기, SaveFileDialog
    private void 저장하기ToolStripMenuItem_Click(object sender, EventArgs e)
    {
      string fileName = DateTime.Now.ToString();
      //foreach (Bokki su in list)
      //{
      //  s += su.X + "," + su.Y + ":" + su.Stone + "\n";
      //}
      MessageBox.Show(fileName);
    }

    // 끝내기 이벤트 처리함수
    private void 끝내기ToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Close();
    }
  }

  // 복기를 위한 클래스
  public class Bokki
  {
    public int X { get; set; }
    public int Y { get; set; }
    public char Stone { set; get; }

    // 생성자
    public Bokki(int x, int y, char stone)
    {
      X = x;
      Y = y;
      Stone = stone;
    }
  }

}
