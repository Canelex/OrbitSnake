using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager instance;
    public string defaultPage;
    public Page[] pages;
    public Field[] fields;
    private Stack<Page> activePages;

    public Field GetField(string name)
    {
        foreach (Field field in fields)
        {
            if (name == field.name)
            {
                return field;
            }
        }

        return null;
    }

    private Page GetPage(string name)
    {
        foreach (Page page in pages)
        {
            if (name == page.name)
            {
                return page;
            }
        }

        return null;
    }

    public void ShowPage(string name)
    {
        Page page = GetPage(name);
        if (page != null)
        {
            page.obj.SetActive(true);
            activePages.Push(page);
        }
        else
        {
            Debug.Log(String.Format("Could not find page {0} to open it.", name));
        }
    }

    public void ClosePage(string name)
    {
        if (activePages.Count == 0) return;

        // Close every page until you close one with name
        for (Page page = activePages.Pop(); activePages.Count >= 1; page = activePages.Pop())
        {
            page.obj.SetActive(false);
            
            if (name == page.name)
            {
                break; // No more
            }
        }
    }

    public void CloseAllPages()
    {
        while (activePages.Count != 0)
        {
            activePages.Pop().obj.SetActive(false);
        }
    }

    public int GetNumPagesOpen()
    {
        return activePages.Count;
    }

    private void Start()
    {
        instance = this;
        activePages = new Stack<Page>();  
        ShowPage(defaultPage); 
    }

    [System.Serializable]
    public class Page
    {
        public string name;
        public GameObject obj;
    }

    [System.Serializable]
    public class Field
    {
        public string name;
        public Text text;
        public Image image;
    }
}
