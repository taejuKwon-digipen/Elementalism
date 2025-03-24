using UnityEngine;
using UnityEditor;
using System.Text;

public class HierarchyExporter
{
    [MenuItem("Tools/Export Hierarchy")]
    static void ExportHierarchy()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("==================== HIERARCHY STRUCTURE ====================");
        
        foreach (GameObject root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        {
            PrintHierarchy(root.transform, 0, sb);
        }
        
        sb.AppendLine("============================================================");
        Debug.Log(sb.ToString());
    }

    static void PrintHierarchy(Transform trans, int indent, StringBuilder sb)
    {
        string indentStr = new string('│', indent) + (indent > 0 ? "├─ " : "");
        string objInfo = GetObjectInfo(trans.gameObject);
        sb.AppendLine(indentStr + objInfo);
        
        for (int i = 0; i < trans.childCount; i++)
        {
            Transform child = trans.GetChild(i);
            if (i == trans.childCount - 1)
            {
                string lastIndentStr = new string('│', indent) + "└─ ";
                PrintHierarchy(child, indent + 1, sb, lastIndentStr);
            }
            else
            {
                PrintHierarchy(child, indent + 1, sb);
            }
        }
    }

    static void PrintHierarchy(Transform trans, int indent, StringBuilder sb, string indentStr)
    {
        string objInfo = GetObjectInfo(trans.gameObject);
        sb.AppendLine(indentStr + objInfo);
        
        for (int i = 0; i < trans.childCount; i++)
        {
            Transform child = trans.GetChild(i);
            if (i == trans.childCount - 1)
            {
                string lastIndentStr = new string('│', indent) + "└─ ";
                PrintHierarchy(child, indent + 1, sb, lastIndentStr);
            }
            else
            {
                PrintHierarchy(child, indent + 1, sb);
            }
        }
    }

    static string GetObjectInfo(GameObject obj)
    {
        StringBuilder info = new StringBuilder();
        
        // 오브젝트 이름
        info.Append(obj.name);
        
        // 비활성화된 경우 표시
        if (!obj.activeInHierarchy)
        {
            info.Append(" [Inactive]");
        }
        
        // 레이어 정보
        if (obj.layer != 0) // Default 레이어가 아닌 경우
        {
            info.Append($" [Layer: {LayerMask.LayerToName(obj.layer)}]");
        }
        
        // 주요 컴포넌트 정보
        var components = obj.GetComponents<Component>();
        if (components.Length > 1) // Transform을 제외한 컴포넌트가 있는 경우
        {
            info.Append(" [");
            bool first = true;
            foreach (var component in components)
            {
                if (component == null) continue;
                if (!first) info.Append(", ");
                info.Append(component.GetType().Name);
                first = false;
            }
            info.Append("]");
        }
        
        return info.ToString();
    }
}
