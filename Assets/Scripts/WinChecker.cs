using UnityEngine;
using System.Collections.Generic;
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
                    bool isReach = CheckWinCondition(grid, x, height, z, opponentPlayerID);
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
    
    // 【変更・追加箇所】 WinChecker.cs にエージェントのリーチ状態（連続した3つのキューブ）の座標を返すメソッドを追加
    public List<Vector3Int> FindAgentReachCubePositions(int[,,] grid, int player)
    {
        List<Vector3Int> reachPositions = new List<Vector3Int>();
        // CheckLine で使用している方向リスト（順序は CheckWinCondition と同じ）
        int[,] directions = new int[,] {
            {1, 0, 0},
            {0, 1, 0},
            {0, 0, 1},
            {1, 1, 0},
            {1, 0, 1},
            {0, 1, 1},
            {-1, 1, 0},
            {-1, 0, 1},
            {0, -1, 1},
            {1, 1, 1},
            {-1, 1, 1},
            {1, 1, -1},
            {1, -1, -1}
        };

        // 盤面は 4x4x4 なので、内部で SIZE (4) を使用
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                for (int z = 0; z < 4; z++)
                {
                    // 各方向について4セル分の連続をチェック
                    for (int d = 0; d < directions.GetLength(0); d++)
                    {
                        int dx = directions[d, 0];
                        int dy = directions[d, 1];
                        int dz = directions[d, 2];

                        List<Vector3Int> positions = new List<Vector3Int>();
                        int countPlayer = 0;
                        int countEmpty = 0;

                        // 4セル分のシーケンスをチェック
                        for (int i = 0; i < 4; i++)
                        {
                            int nx = x + i * dx;
                            int ny = y + i * dy;
                            int nz = z + i * dz;

                            // 範囲外ならこの方向は無効
                            if (nx < 0 || nx >= 4 || ny < 0 || ny >= 4 || nz < 0 || nz >= 4)
                            {
                                positions = null;
                                break;
                            }
                            positions.Add(new Vector3Int(nx, ny, nz));
                            int cell = grid[nx, ny, nz];
                            if (cell == player)
                            {
                                countPlayer++;
                            }
                            else if (cell == 0 || cell == -2) // EMPTY または DANGER とみなす
                            {
                                // **修正箇所**: そのセルが実際に利用可能かチェック（その列の GetAvailableHeight と比較）
                                int availableY = GridManager.Instance.GetAvailableHeight(nx, nz);
                                if (availableY == ny)
                                {
                                    countEmpty++;
                                }
                                else
                                {
                                    // 利用可能高さと一致しない場合は、この並びは有効なリーチではない
                                    positions = null;
                                    break;
                                }
                            }
                            else
                            {
                                // 相手の駒があれば無効
                                positions = null;
                                break;
                            }
                        }
                        if (positions != null && countPlayer == 3 && countEmpty == 1)
                        {
                            // 連続している3つの駒の位置のみを返す（最初に見つかったリーチライン）
                            List<Vector3Int> agentCubes = new List<Vector3Int>();
                            foreach (var pos in positions)
                            {
                                if (grid[pos.x, pos.y, pos.z] == player)
                                {
                                    Debug.Log("x: " + pos.x + ", y: " + pos.y + ", z: " + pos.z);
                                    agentCubes.Add(pos);
                                }
                            }
                            Debug.Log("first: " + agentCubes[0] + ", second: " + agentCubes[1] + ", third: " + agentCubes[2]);
                            return agentCubes;
                        }
                    }
                }
            }
        }
        return reachPositions;
    }



}