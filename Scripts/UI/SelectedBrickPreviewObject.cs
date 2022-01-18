using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedBrickPreviewObject : MonoBehaviour
{
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
    }
    private void FixedUpdate()
    {
        UpdateSelectedBrick();
    }
    public void UpdateSelectedBrick()
    {
        if (EditorUIManager.Instance == null) return;
        if (EditorUIManager.Instance.cubeDesigneInstance == null) return;
        if (EditorUIManager.Instance.cubeDesigneInstance.isCursorExists == false) return;
        meshFilter.mesh = EditorUIManager.Instance.cubeDesigneInstance.GetCursorMesh();
        meshRenderer.material = EditorUIManager.Instance.cubeDesigneInstance.GetCursorMaterial();
    }
}
