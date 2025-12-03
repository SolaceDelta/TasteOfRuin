using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class DungeonGenerator : MonoBehaviour
{
    // Rooms
    public GameObject startingI;
    public GameObject starting;
    public GameObject basic;
    public GameObject empty;
    public GameObject heal;
    public GameObject loot;
    public GameObject shop;
    public GameObject bossChamber;

    // Wall & Floor / Ceiling
    public GameObject wall;
    public GameObject door;
    public GameObject roof;

    // Player & Camera
    public GameObject player;

    // Data representation of the dungeon:
    private bool[,] dDungeon = new bool[9, 9];
    private (bool, bool, bool, bool)[,] filled = new (bool, bool, bool, bool)[9, 9];

    // Dungeon Container
    private Transform dungeon;
    
    // Center of the Dungeon Grid with respect to room "centers"
    private Vector3 center = new Vector3(113f, 102f, -1f);
    private Vector3 pointer;
    
    // Offsets
    private Vector3 left  = new Vector3(-25f, 0f, 0f);
    private Vector3 up    = new Vector3(0f, 25f, 0f);
    private Vector3 right = new Vector3(25f, 0f, 0f);
    private Vector3 down  = new Vector3(0f, -25f, 0f);

    // Prefab Offsets for filling walls
    private Vector3 ceiling = new Vector3(0f, 23.5f, 0f);
    private Vector3 floor   = new Vector3(0f, -1.5f, 0f);
    private Vector3 leftW   = new Vector3(-12.5f, 0f, 0f);
    private Vector3 rightW  = new Vector3(12.5f, 0f, 0f);

    // String Constants
    private const string L = "LEFT";
    private const string U = "UP";
    private const string R = "RIGHT";
    private const string D = "DOWN";

    // Opposite directions
    Dictionary<string, string> opposite = new Dictionary<string, string>
    {
        {L, R},
        {U, D},
        {R, L},
        {D, U}
    };
    
    // Limits
    private (int min, int max) shopLimit = (1, 4);
    private (int min, int max) lootLimit = (4, 10);
    private (int min, int max) healLimit = (3, 12);
    private (int min, int max) roomLimit = (18, 81);
    
    private List<string> directions = new List<string> {L, U, R, D};
    private List<string> validDirections = new List<string>();
    private List<GameObject> possibleRooms;
    private int rooms = 0, shops = 0, loots = 0, heals = 0;
    private bool boss = false;
    private string backwards = "";
    private List<(int, int)> bossCoords = new List<(int, int)>();

    void Start()
    {
        possibleRooms = new List<GameObject> {basic, empty, heal, loot, shop};
        dungeon = GameObject.Find("Dungeon").transform;
        GenerateDungeon();
    }

    private void GenerateDungeon()
    {
        rooms = 1;
        pointer = center;

        Instantiate(startingI, center, Quaternion.identity, dungeon);
        Instantiate(player, center + new Vector3(0f, 4f, 0f), Quaternion.identity);
        
        dDungeon[4, 4] = true;
        GameObject girl = GameObject.Find("Girl");
        AttributeController attr = girl.GetComponent<AttributeController>();
        int level = attr.attr.PERSISTENT.level;

        LevelLimits(level);
        attr.BeginRun();
        
        while (rooms < roomLimit.max)
        {
            if (MinimumMet() && Random.Range(0, 25) == 0) break;
            int dirs = (rooms < roomLimit.min) ? Random.Range(1, 5) : Random.Range(0, 5);
            if (dirs == 4) validDirections.AddRange(directions);
            else if (dirs == 3)
            {
                List<int> idx = new List<int> {0, 1, 2, 3};
                idx.RemoveAt(Random.Range(0, 4));
                validDirections.AddRange(new List<string> {directions[idx[0]], directions[idx[1]], directions[idx[2]]});
            }
            else if (dirs == 2)
            {
                List<int> idx = new List<int> {0, 1, 2, 3};
                idx.RemoveAt(Random.Range(0, 4));
                idx.RemoveAt(Random.Range(0, 3));
                validDirections.AddRange(new List<string> {directions[idx[0]], directions[idx[1]]});
            }
            else if (dirs == 1) validDirections.Add(directions[Random.Range(0, 4)]);
            // Reset for next loop, choose direction to move
            PlaceRooms();
            LoopCleanup();
        }
        FillHoles();
        Debug.Log("FINISHED LOOPING!");
    }

    private void PlaceRooms()
    {
        PlaceLeft();
        PlaceUp();
        PlaceRight();
        PlaceDown();
    }

    private void PlaceLeft()
    {
        Vector3 loc = pointer + left;
        (int, int) coords = ConvertCoords(loc);
        int x = coords.Item1, x1 = x - 1, y = coords.Item2, y1 = y + 1, y2 = y - 1;

        if (validDirections.Contains(L) && CellInBounds(x, y) && !dDungeon[x, y])
        {
            GameObject room = RandomRoom();

            if (room == bossChamber)
            {
                var options = new[] {(left,        (-1,  1)), (left + down, (-1, -1))};

                if (!TryPlaceBossChamber(loc, x, y, options))
                {
                    possibleRooms.Remove(bossChamber);
                    room = RandomRoom();
                }
            }
            if (room != bossChamber) PlaceRoom(room, loc, x, y);
        }
        if (backwards != L && (!CellInBounds(x, y) || !dDungeon[x, y] || bossCoords.Contains((x, y)))) validDirections.Remove(L);
    }

    private void PlaceUp()
    {
        Vector3 loc = pointer + up;
        (int, int) coords = ConvertCoords(loc);
        int x = coords.Item1, x1 = x - 1, x2 = x + 1, y = coords.Item2, y1 = y + 1;

        if (validDirections.Contains(U) && CellInBounds(x, y) && !dDungeon[x, y])
        {
            GameObject room = RandomRoom();

            if (room == bossChamber)
            {
                var options = new[] {(left,         (-1, 1)), (Vector3.zero, ( 1, 1))};
                
                if (!TryPlaceBossChamber(loc, x, y, options))
                {
                    possibleRooms.Remove(bossChamber);
                    room = RandomRoom();
                }
            }
            if (room != bossChamber) PlaceRoom(room, loc, x, y);
        }
        if (backwards != U && (!CellInBounds(x, y) || !dDungeon[x, y] || bossCoords.Contains((x, y)))) validDirections.Remove(U);
    }
    
    private void PlaceRight()
    {
        Vector3 loc = pointer + right;
        (int, int) coords = ConvertCoords(loc);
        int x = coords.Item1, x1 = x + 1, y = coords.Item2, y1 = y + 1, y2 = y - 1;

        if (validDirections.Contains(R) && CellInBounds(x, y) && !dDungeon[x, y])
        {
            GameObject room = RandomRoom();

            if (room == bossChamber)
            {
                var options = new[] {(Vector3.zero, (1,  1)), (down,         (1, -1))};
                
                if (!TryPlaceBossChamber(loc, x, y, options))
                {
                    possibleRooms.Remove(bossChamber);
                    room = RandomRoom();
                }
            }
            if (room != bossChamber) PlaceRoom(room, loc, x, y);
        }
        if (backwards != R && (!CellInBounds(x, y) || !dDungeon[x, y] || bossCoords.Contains((x, y)))) validDirections.Remove(R);
    }
    
    private void PlaceDown()
    {
        Vector3 loc = pointer + down;
        (int, int) coords = ConvertCoords(loc);
        int x = coords.Item1, x1 = x - 1, x2 = x + 1, y = coords.Item2, y1 = y + 1;

        if (validDirections.Contains(D)  && CellInBounds(x, y) && !dDungeon[x, y])
        {
            GameObject room = RandomRoom();

            if (room == bossChamber)
            {
                var options = new[] {(left,         (-1,  1)), (Vector3.zero, ( 1,  1))};

                if (!TryPlaceBossChamber(loc, x, y, options))
                {
                    possibleRooms.Remove(bossChamber);
                    room = RandomRoom();
                }
            }
            if (room != bossChamber) PlaceRoom(room, loc, x, y);
        }
        if (backwards != D && (!CellInBounds(x, y) || !dDungeon[x, y] || bossCoords.Contains((x, y)))) validDirections.Remove(D);
    }

    private bool TryPlaceBossChamber(Vector3 loc, int x, int y, (Vector3 offset, (int dx, int dy) gridOffset)[] options)
    {
        foreach (var (offset, gridOffset) in options)
        {
            int x1 = x + gridOffset.dx;
            int y1 = y + gridOffset.dy;

            if (AreaFree(x, x1, y, y1))
            {
                Instantiate(bossChamber, loc + offset, Quaternion.identity, dungeon);
                dDungeon[ x,  y] = true;
                dDungeon[x1,  y] = true;
                dDungeon[ x, y1] = true;
                dDungeon[x1, y1] = true;

                bossCoords.Add(( x,  y));
                bossCoords.Add((x1,  y));
                bossCoords.Add(( x, y1));
                bossCoords.Add((x1, y1));

                boss = true;
                possibleRooms.Remove(bossChamber);

                Debug.Log($"Placing: Boss Chamber At: {ConvertCoords(loc + offset)}, {loc + offset}");
                Debug.Log($"Possible Rooms: {possibleRooms}");
                return true;
            }
        }
        return false;
    }

    private GameObject RandomRoom()
    {
        GameObject room = possibleRooms[Random.Range(0, possibleRooms.Count)];
        if (room == shop)
        {
            shops++;
            if (shops == shopLimit.max) possibleRooms.Remove(shop);
        }
        else if (room == loot)
        {
            loots++;
            if (loots == lootLimit.max) possibleRooms.Remove(loot);
        }
        else if (room == heal)
        {
            heals++;
            if (heals == healLimit.max) possibleRooms.Remove(heal);
        }
        rooms++;

        if (rooms >= roomLimit.min && !boss && !possibleRooms.Contains(bossChamber)) possibleRooms.Add(bossChamber);
        return room;
    }

    private void PlaceRoom(GameObject room, Vector3 loc, int x, int y)
    {
        if (room == shop) filled[x, y].Item4 = true;
        Instantiate(room, loc, Quaternion.identity, dungeon);
        dDungeon[x, y] = true;
        Debug.Log($"Placing: {room} At: {ConvertCoords(loc)}, {loc}");
        Debug.Log($"Possible Rooms: {possibleRooms}");   
    }

    private void LoopCleanup()
    {
        (int lx, int ly) = ConvertCoords(pointer + left);
        if (!validDirections.Contains(L) && CellInBounds(lx, ly) && dDungeon[lx, ly] && !bossCoords.Contains((lx, ly))) validDirections.Add(L);
        (int ux, int uy) = ConvertCoords(pointer + up);
        if (!validDirections.Contains(U) && CellInBounds(ux, uy) && dDungeon[ux, uy] && !bossCoords.Contains((ux, uy))) validDirections.Add(U);
        (int rx, int ry) = ConvertCoords(pointer + right);
        if (!validDirections.Contains(R) && CellInBounds(rx, ry) && dDungeon[rx, ry] && !bossCoords.Contains((rx, ry))) validDirections.Add(R);
        (int dx, int dy) = ConvertCoords(pointer + down);
        if (!validDirections.Contains(D) && CellInBounds(dx, dy) && dDungeon[dx, dy] && !bossCoords.Contains((dx, dy))) validDirections.Add(D);
        
        Debug.Log(validDirections.Count);
        string toMove = validDirections[Random.Range(0, validDirections.Count)];
        backwards = opposite[toMove];
        
        if (toMove == L) pointer += left;
        else if (toMove == U) pointer += up;
        else if (toMove == R) pointer += right;
        else if (toMove == D) pointer += down;
        else Debug.LogError("|ERROR| No valid direction chosen");
        
        Debug.Log($"Moving to: {ConvertCoords(pointer)}, {pointer}");
        validDirections = new List<string> {backwards};
    }

    private void FillHoles()
    {
        for (int i = 0; i < dDungeon.GetLength(0); i++)
        {
            for (int j = 0; j < dDungeon.GetLength(1); j++)
            {
                if (dDungeon[i, j] && !bossCoords.Contains((i, j))) FillDivisons(i, j, new string[4] {L, U, R, D});
                else if (bossCoords.Contains((i, j)))
                {
                    (int, int) dl = (0, 0), dr = (0, 0), ul = (0, 0), ur = (0, 0);

                    int minX = bossCoords.Min(c => c.Item1);
                    int minY = bossCoords.Min(c => c.Item2);

                    foreach ((int, int) c in bossCoords)
                    {
                        (int, int) diff = (c.Item1 - minX, c.Item2 - minY);

                        if (diff == (0, 0)) dl = c;
                        else if (diff == (1, 0)) dr = c;
                        else if (diff == (0, 1)) ul = c;
                        else if (diff == (1, 1)) ur = c;
                    }
                    // DL gets left and bottom
                    FillDivisons(dl.Item1, dl.Item2, new string[2] {L, D});
                    // DR gets right and bottom
                    FillDivisons(dr.Item1, dr.Item2, new string[2] {R, D});
                    // UL gets left and top
                    FillDivisons(ul.Item1, ul.Item2, new string[2] {L, U});
                    // UR gets right and top
                    FillDivisons(ur.Item1, ur.Item2, new string[2] {R, U});
                }
            }
        }
    }

    private void FillDivisons(int x, int y, string[] dirs)
    {
        (bool, bool, bool, bool) f = filled[x, y];
        bool inBounds;
        
        Vector3 loc = ConvertTransform(x, y);
        GameObject filler;

        foreach (string dir in dirs)
        {
            if (dir == L && !f.Item1)
            {
                inBounds = CellInBounds(x - 1, y);
                filler = (!inBounds || (inBounds && !dDungeon[x - 1, y])) ? wall : door;
                
                Instantiate(filler, loc + leftW, Quaternion.identity, dungeon);
                
                filled[x, y].Item1 = true;
                if (inBounds) filled[x - 1, y].Item3 = true;
            }
            else if (dir == U && !f.Item2 && (!CellInBounds(x, y + 1) || (CellInBounds(x, y + 1) && !dDungeon[x, y + 1])))
            {
                Instantiate(roof, loc + ceiling, Quaternion.identity, dungeon);
                
                filled[x, y].Item2 = true;
                if (CellInBounds(x, y + 1)) filled[x, y + 1].Item4 = true;
            }
            else if (dir == R && !f.Item3)
            {
                inBounds = CellInBounds(x + 1, y);
                filler = (!inBounds || (inBounds && !dDungeon[x + 1, y])) ? wall : door;
                
                Instantiate(filler, loc + rightW, Quaternion.identity, dungeon);

                filled[x, y].Item3 = true;
                if (inBounds) filled[x + 1, y].Item1 = true;
            }
            else if (dir == D && !f.Item4 && (!CellInBounds(x, y - 1) || (CellInBounds(x, y - 1) && !dDungeon[x, y - 1])))
            {
                Instantiate(roof, loc + floor, Quaternion.identity, dungeon);
                
                filled[x, y].Item4 = true;
                if (CellInBounds(x, y - 1)) filled[x, y - 1].Item2 = true;
            }
        }
    }

    private void LevelLimits(int l)
    {
        int ten    = (int) (l * 0.1); // Every 10 levels 4 40
        int twenty = (int) (l * 0.2); // Every 5 levels 10 50
        int fifty  = (int) (l * 0.5); // Every 2 levels 12 6
        shopLimit.min = Mathf.Max(0, shopLimit.min - ten);
        shopLimit.max = Mathf.Max(1, shopLimit.max - ten);
        lootLimit.min = Mathf.Max(0, lootLimit.min - twenty);
        lootLimit.max = Mathf.Max(1, lootLimit.max - twenty);
        healLimit.min = Mathf.Max(0, healLimit.min - fifty);
        healLimit.max = Mathf.Max(1, healLimit.max - fifty);
        roomLimit.min = Mathf.Min(roomLimit.max, roomLimit.min + fifty);
    }
    private bool AreaFree(int x, int x1, int y, int y1) {return CellInBounds(x1, y1) && !dDungeon[x1, y] && !dDungeon[x, y1] && !dDungeon[x1, y1];}
    private bool MinimumMet() {return rooms >= roomLimit.min && shops >= shopLimit.min && loots >= lootLimit.min && heals >= healLimit.min && boss;}
    private (int, int) ConvertCoords(Vector3 coords) {return (4 + (int)(coords.x - center.x) / 25, 4 + (int)(coords.y - center.y) / 25);}
    private Vector3 ConvertTransform(int x, int y) {return center + new Vector3((float)(x - 4) * 25, (float)(y - 4) * 25, 0f);}
    private bool CellInBounds(int x, int y) {return x <= 8 && x >= 0 && y <= 8 && y >= 0;}
}