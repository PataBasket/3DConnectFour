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
    
    // リーチ（3つ並べる）の検出
    public bool CheckReachCondition(int[,,] grid, int x, int y, int z, int player)
    {
        // 方向の組み合わせをすべてチェック
        int[][] directions = new int[][]
        {
            new int[] {1, 0, 0},
            new int[] {0, 1, 0},
            new int[] {0, 0, 1},
            new int[] {1, 1, 0},
            new int[] {1, 0, 1},
            new int[] {0, 1, 1},
            new int[] {-1, 1, 0},
            new int[] {-1, 0, 1},
            new int[] {0, -1, 1},
            new int[] {1, 1, 1},
            new int[] {-1, 1, 1},
            new int[] {1, 1, -1},
            new int[] {1, -1, -1}
        };

        foreach (var dir in directions)
        {
            if (CheckLineForReach(grid, x, y, z, dir[0], dir[1], dir[2], player))
            {
                return true;
            }
        }
        return false;
    }

    private bool CheckLineForReach(int[,,] grid, int x, int y, int z, int dx, int dy, int dz, int player)
    {
        int count = 0;
        int emptyCount = 0;

        for (int i = -3; i <= 3; i++)
        {
            int nx = x + i * dx;
            int ny = y + i * dy;
            int nz = z + i * dz;

            if (nx >= 0 && nx < GridManager.SIZE && ny >= 0 && ny < GridManager.HEIGHT && nz >= 0 && nz < GridManager.SIZE)
            {
                int cellValue = grid[nx, ny, nz];
                if (cellValue == player)
                {
                    count++;
                }
                else if (cellValue == 0)
                {
                    emptyCount++;
                }
                else
                {
                    // 相手のキューブがある場合、リーチは成立しないのでリセット
                    count = 0;
                    emptyCount = 0;
                }

                if (count == 3 && emptyCount >= 1)
                {
                    return true;
                }
            }
            else
            {
                count = 0;
                emptyCount = 0;
            }
        }
        return false;
    }
    
    // 相手のリーチ（3つ並べた状態）を検出してその位置を返す
    public (int, int) FindOpponentReach(int[,,] grid, int opponentPlayerID)
    {
        for (int x = 0; x < GridManager.SIZE; x++)
        {
            for (int z = 0; z < GridManager.SIZE; z++)
            {
                int height = GridManager.Instance.GetAvailableHeight(x, z);
                if (height != -1)
                {
                    // 仮にこの場所に相手が置いた場合リーチが完成するかどうか
                    grid[x, height, z] = opponentPlayerID; // 仮に置く
                    bool isReach = CheckReachCondition(grid, x, height, z, opponentPlayerID);
                    grid[x, height, z] = 0; // 元に戻す

                    if (isReach)
                    {
                        // リーチが完成する場所が見つかったので、その位置を返す
                        return (x, z);
                    }
                }
            }
        }
        return (-1, -1); // リーチがない場合
    }

}