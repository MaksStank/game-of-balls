using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace game3
{
    public partial class Form1 : Form
    {
        private Random random = new Random();
        private int score = 0;
        private int platformWidth = 80;
        private int platformHeight = 15;
        private int platformX;
        private int ballSize = 30;
        private Timer gameTimer;
        private List<Ball> balls = new List<Ball>();
        private bool gameOver = false;
        private int gameSpeed = 10;

        public Form1()
        {

            this.DoubleBuffered = true;
            this.ClientSize = new Size(600, 600);
            this.Text = "Ловец шаров";

            platformX = (this.ClientSize.Width - platformWidth) / 2;

            gameTimer = new Timer();
            gameTimer.Interval = 50;
            gameTimer.Tick += GameLoop;
            gameTimer.Start();

            this.KeyDown += Form1_KeyDown;
            this.Paint += Form1_Paint;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameOver)
            {
                if (e.KeyCode == Keys.R)
                {
                    // Перезапуск игры
                    score = 0;
                    balls.Clear();
                    gameOver = false;
                    gameTimer.Interval = 50;
                    gameSpeed = 10;
                    gameTimer.Start();
                    this.Invalidate();
                }
                return;
            }

            int step = 20;

            if (e.KeyCode == Keys.Left && platformX > 0)
            {
                platformX -= step;
                if (platformX < 0) platformX = 0;
            }
            else if (e.KeyCode == Keys.Right && platformX < this.ClientSize.Width - platformWidth)
            {
                platformX += step;
                if (platformX > this.ClientSize.Width - platformWidth)
                    platformX = this.ClientSize.Width - platformWidth;
            }

            this.Invalidate();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Рисуем платформу
            g.FillRectangle(Brushes.Blue, platformX, this.ClientSize.Height - 30, platformWidth, platformHeight);

            // Рисуем шары
            foreach (var ball in balls)
            {
                g.FillEllipse(ball.Color == BallColor.White ? Brushes.White : Brushes.Red,
                              ball.X, ball.Y, ballSize, ballSize);
            }

            // Рисуем счет
            g.DrawString($"Счет: {score}", new Font("Arial", 14), Brushes.Black, 10, 10);

            if (gameOver)
            {
                g.DrawString("Игра окончена!", new Font("Arial", 24), Brushes.Red,
                            this.ClientSize.Width / 2 - 100, this.ClientSize.Height / 2 - 30);
                g.DrawString($"Финальный счет: {score}", new Font("Arial", 18), Brushes.Black,
                            this.ClientSize.Width / 2 - 80, this.ClientSize.Height / 2 + 20);
                g.DrawString("Нажмите R для перезапуска", new Font("Arial", 12), Brushes.Gray,
                            this.ClientSize.Width / 2 - 100, this.ClientSize.Height / 2 + 60);
            }
        }

        private void GameLoop(object sender, EventArgs e)
        {
            if (gameOver) return;

            // Случайное добавление новых шаров
            if (random.Next(0, 100) < gameSpeed)
            {
                BallColor color = random.Next(0, 2) == 0 ? BallColor.White : BallColor.Red;
                balls.Add(new Ball
                {
                    X = random.Next(0, this.ClientSize.Width - ballSize),
                    Y = 0,
                    Color = color
                });
            }

            // Движение шаров
            for (int i = balls.Count - 1; i >= 0; i--)
            {
                balls[i].Y += 5;

                // Проверка столкновения с платформой
                if (balls[i].Y + ballSize >= this.ClientSize.Height - 30 &&
                    balls[i].Y + ballSize <= this.ClientSize.Height - 15 &&
                    balls[i].X + ballSize >= platformX &&
                    balls[i].X <= platformX + platformWidth)
                {
                    if (balls[i].Color == BallColor.White)
                    {
                        score++;
                        balls.RemoveAt(i);
                    }
                    else
                    {
                        gameOver = true;
                        gameTimer.Stop();
                    }
                }

                // Удаление шаров, ушедших за экран
                if (balls[i].Y > this.ClientSize.Height)
                {
                    balls.RemoveAt(i);
                }
            }

            // Постепенное увеличение сложности
            if (score > 0 && score % 10 == 0 && gameTimer.Interval > 20)
            {
                gameTimer.Interval -= 1;
                gameSpeed += 1;
            }

            this.Invalidate();
        }
    }

    public enum BallColor { White, Red }

    public class Ball
    {
        public int X { get; set; }
        public int Y { get; set; }
        public BallColor Color { get; set; }
    }
}