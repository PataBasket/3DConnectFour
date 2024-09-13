public class WinChecker
{
    private const int SIZE = 4;

    // 勝利条件をチェック
    public bool CheckWinCondition(int[,,] grid, int x, int y, int z, int player)
    {
        return CheckLine(grid, x, y, z, 1, 0, 0, player) || // x方向
               CheckLine(grid, x, y, z, 0, 1, 0, player) || // y方向
               CheckLine(grid, x, y, z, 0, 0, 1, player) || // z方向
               CheckLine(grid, x, y, z, 1, 1, 0, player) || // xy斜め
               CheckLine(grid, x, y, z, 1, 0, 1, player) || // xz斜め
               CheckLine(grid, x, y, z, 0, 1, 1, player) || // yz斜め
               CheckLine(grid, x, y, z, -1, 1, 0, player) || // xy逆斜め
               CheckLine(grid, x, y, z, -1, 0, 1, player) || // xz逆斜め
               CheckLine(grid, x, y, z, 0, -1, 1, player) || // yz逆斜め
               CheckLine(grid, x, y, z, 1, 1, 1, player) || // xyz斜め
               CheckLine(grid, x, y, z, -1, 1, 1, player) || // xyz逆斜め
               CheckLine(grid, x, y, z, 1, 1, -1, player) || // xyz逆斜め
               CheckLine(grid, x, y, z, 1, -1, -1, player);  // xyz逆方向の逆斜め
    }

    // 指定した方向に連続したキューブが4つ揃っているかをチェック
    private bool CheckLine(int[,,] grid, int x, int y, int z, int dx, int dy, int dz, int player)
    {
        int count = 0;
        for (int i = -3; i <= 3; i++)
        {
            int nx = x + i * dx;
            int ny = y + i * dy;
            int nz = z + i * dz;

            if (nx >= 0 && nx < SIZE && ny >= 0 && ny < SIZE && nz >= 0 && nz < SIZE && grid[nx, ny, nz] == player)
            {
                count++;
                if (count == 4) return true;
            }
            else
            {
                count = 0;
            }
        }
        return false;
    }
}