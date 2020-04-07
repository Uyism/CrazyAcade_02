using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JUtils
{
    static public Const.EDirection GetDir(Vector3 target_pos, Vector3 cur_pos)
    {
        Vector3 dir = target_pos - cur_pos;
        Const.EDirection e_dir;
        if (Mathf.Abs(dir.y) < Mathf.Abs(dir.x))
        {
            if (dir.x > 0) e_dir = Const.EDirection.Right;
            else e_dir = Const.EDirection.Left;
        }
        else
        {
            if (dir.y > 0) e_dir = Const.EDirection.UP;
            else e_dir = Const.EDirection.Down;
        }

        return e_dir;
    }
}
