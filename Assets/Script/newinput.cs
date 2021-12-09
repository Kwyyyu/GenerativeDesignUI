using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Linq;
using System.IO;
using System.Text;


public class newinput : MonoBehaviour
{
    public class Constraint{
        public bool attach_to_vertical_plane = false;
        public bool attach_to_horizontal_plane = false;
        public List<GameObject> same_vertical_plane_with = new List<GameObject>();
        public List<GameObject> same_horizontal_plane_with = new List<GameObject>();
        public List<GameObject> align_x_with = new List<GameObject>();
        public List<GameObject> align_y_with = new List<GameObject>();
        public List<GameObject> align_z_with = new List<GameObject>();
    }
    public GameObject wall;
    public GameObject floor;
    public GameObject desk;
    public GameObject clock;
    public GameObject window;
    private GameObject MovingObject;
    private Camera cam;
    public int whichobject = 3;
    GameObject prefab;
    public Button Wall_B;
    public Button Floor_B;
    public Button Desk_B;
    public Button Clock_B;
    public Button Window_B;
    public Button EditMode;
    public Button Generate_B;
    public Button Close_B;
    public Button Close_B2;
    public Button Export_B;
    public GameObject Constraint_Panel;
    public Button Show_Constraint_B;
    public Text Constraint_Text;
    public GameObject Destory_Q;
    public Text EditeMode_T;
    private Text[] Temp_T;
    public Material Click_m;
    public Material Unclick_m;
    public Material Unclick_go_m;
    public int EditModeOn = 0;
    public int ClickCount = 0;
    public bool Destroy_A = false;
    bool Destroy_B = false;
    public Slider floor_slider_x;
    public Slider floor_slider_y;
    public Slider wall_slider;
    public GameObject BaseFloor;
    public GameObject bool_wall_slider;
    float wall_width;
    GameObject wall_adj;
    private int wallCount = 0;
    private int floorCount = 0;
    private int clockCount = 0;
    private int deskCount = 0;
    private int windowCount = 0;
    public Dictionary<GameObject, List<GameObject>> plane_dict;
    public Dictionary<GameObject, Constraint> object_dict;
    public List<string> constraints;
    // align
    public GameObject alignedObj;
    public Dictionary<string, List<GameObject>> alignDict;
    public GameObject Align_Panel;
    public Text Align_Text;
    public Button Align_Yes;
    public Button Align_No;

    void Start()
    {
        whichobject = default;
        cam = Camera.main;
        Wall_B.onClick.AddListener(wClick);
        Floor_B.onClick.AddListener(fClick);
        Desk_B.onClick.AddListener(dClick);
        Clock_B.onClick.AddListener(cClick);
        Window_B.onClick.AddListener(winClick);
        EditMode.onClick.AddListener(EClick);
        floor_slider_x.value = 1.0f;
        floor_slider_y.value = 1.0f;
        wall_slider.value = 1.0f;
        floor_slider_x.onValueChanged.AddListener(fSlider_x);
        floor_slider_y.onValueChanged.AddListener(fSlider_y);
        wall_slider.onValueChanged.AddListener(wallslider);
        BaseFloor.GetComponent<cakeslice.Outline>().enabled = false;
        //BaseFloor.GetComponent<Outline>().enabled = false;
        Generate_B.onClick.AddListener(gClick);
        Close_B.onClick.AddListener(closePanel);
        Close_B2.onClick.AddListener(closePanel);
        Export_B.onClick.AddListener(exportClick);
        Show_Constraint_B.onClick.AddListener(sClick);
        Constraint_Panel.SetActive(false);
        Show_Constraint_B.gameObject.SetActive(false);
        plane_dict = new Dictionary<GameObject, List<GameObject>>();
        object_dict = new Dictionary<GameObject, Constraint>();
        // align
        alignDict = new Dictionary<string, List<GameObject>>();
        alignDict["align_x"] = new List<GameObject>();
        alignDict["align_y"] = new List<GameObject>();
        alignDict["align_z"] = new List<GameObject>();
        Align_Panel.SetActive(false);
        Align_Yes.onClick.AddListener(alignYesClick);
        Align_No.onClick.AddListener(alignNoClick);
    }

    void Update()
    {
        switch (whichobject)
        {
            case 1:
                prefab = wall;
                break;

            case 2:
                prefab = floor;
                break;

            case 3:
                prefab = desk;
                break;

            case 4:
                prefab = clock;
                break;

            case 5:
                prefab = window;
                break;

            default:
                prefab = null;
                break;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame && Constraint_Panel.activeSelf==false && Align_Panel.activeSelf == false)
        {
            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            // add object/plane to the scene
            if (prefab != null)
            {
                if (Physics.Raycast(ray, out hit) && ClickCount == 0) //only for physical gameObjects
                {
                    if (hit.transform.gameObject.name.Contains("Wall"))
                    {
                        if (prefab == clock || prefab == desk || prefab == window){
                            GameObject obj = null;
                            if (hit.point.x < -4*floor_slider_x.value || hit.point.x > 4 * floor_slider_x.value){
                                if (hit.point.x < -4)
                                { 
                                    obj = Instantiate(prefab, new Vector3(hit.point.x, hit.point.y + prefab.transform.position.y, hit.point.z), Quaternion.Euler(new Vector3(-90, -90, 0)));
                                    //prefabobject.transform.parent = hit.transform.gameObject.transform;
                                    print("111");
                                    changeName(obj, prefab);
                                }
                                else
                                {
                                    obj = Instantiate(prefab, new Vector3(hit.point.x, hit.point.y + prefab.transform.position.y, hit.point.z), Quaternion.Euler(new Vector3(-90, 90, 0)));
                                    //prefabobject.transform.parent = hit.transform.gameObject.transform;
                                    print("222");
                                    changeName(obj, prefab);
                                }
                            }
                            else{
                                obj = Instantiate(prefab, new Vector3(hit.point.x, hit.point.y + prefab.transform.position.y, hit.point.z), Quaternion.Euler(new Vector3(-90, 0, 0)));
                                //prefabobject.transform.parent = hit.transform.gameObject.transform;
                                print("333");
                                changeName(obj, prefab);
                            }
                            print(hit.point);
                            whichobject = default;
                            addItem(obj, true, hit.transform.gameObject);
                        }
                    }
                    else if (hit.transform.gameObject.name.Contains("Plane")){
                        if (prefab == wall || prefab == floor){
                            GameObject obj = null;
                            if (hit.point.x < -4 * floor_slider_x.value || hit.point.x > 4 * floor_slider_x.value)
                            {
                                obj = Instantiate(prefab, new Vector3(hit.point.x, hit.point.y + prefab.transform.position.y, hit.point.z), Quaternion.Euler(new Vector3(0, 90, 0)));
                                print("444");
                                changeName(obj, prefab);
                            }
                            else
                            {
                                //Instantiate(prefab, hit.point, Quaternion.identity); without height
                                obj = Instantiate(prefab, new Vector3(hit.point.x, hit.point.y + prefab.transform.position.y, hit.point.z), Quaternion.identity);
                                print("555");
                                changeName(obj, prefab);
                            }

                            plane_dict[obj] = new List<GameObject>();
                            print(hit.point);
                            whichobject = default;
                        }
                    }
                    else if (hit.transform.gameObject.name.Contains("Floor"))
                    {
                        if (prefab == clock || prefab == desk || prefab == window){
                            
                            GameObject obj = null;
                            if (hit.point.x < -4 * floor_slider_x.value || hit.point.x > 4 * floor_slider_x.value)
                            {
                                obj = Instantiate(prefab, new Vector3(hit.point.x, hit.point.y + prefab.transform.position.y, hit.point.z), Quaternion.Euler(new Vector3(0, 90, 0)));
                                print("666");
                                changeName(obj, prefab);
                            }
                            else
                            {
                                //Instantiate(prefab, hit.point, Quaternion.identity); without height
                                obj = Instantiate(prefab, new Vector3(hit.point.x, hit.point.y + prefab.transform.position.y, hit.point.z), Quaternion.identity);
                                print("777");
                                changeName(obj, prefab);
                            }
                            print(hit.point);
                            whichobject = default;
                            addItem(obj, false, hit.transform.gameObject);
                            
                        }
                    }
                }
            }

            if (prefab == null)
            {
                if (Physics.Raycast(ray, out hit) && ClickCount == 0) {
                    if (hit.transform.gameObject.tag == "OPrefab") {
                        Show_Constraint_B.gameObject.SetActive(true);
                        MovingObject = hit.transform.gameObject;
                        unhighlightAll();
                        highlightObj();                        
                    } else {
                        Show_Constraint_B.gameObject.SetActive(false);
                        unhighlightAll();
                        MovingObject = null;
                    }

                }

                if (Physics.Raycast(ray, out hit) && EditModeOn>=1 && ClickCount != 0 && Destroy_B == false) 
                {
                    if (hit.transform.gameObject.tag == "Prefab" || hit.transform.gameObject.tag == "OPrefab")
                    {
                        EditModeOn = 2;
                    }


                    if (EditModeOn == 2 && ClickCount == 1) //for move
                    {
                        if (hit.transform.gameObject.tag == "OPrefab")
                        {
                            MovingObject = hit.transform.gameObject;
                            EditModeOn = 2;
                            unhighlightAll();
                            highlightObj();
                        }
                        else if (hit.transform.gameObject.tag == "Prefab")
                        {
                            if(MovingObject != null && MovingObject.gameObject.tag == "OPrefab"){
                                if (MovingObject.gameObject.name.Contains("Desk"))
                                { 
                                    MovingObject.transform.position = new Vector3(hit.point.x, hit.point.y + desk.transform.position.y, hit.point.z); 
                                }

                                if (MovingObject.gameObject.name.Contains("Window"))
                                { 
                                    MovingObject.transform.position = new Vector3(hit.point.x, hit.point.y + window.transform.position.y, hit.point.z); 
                                }

                                if (MovingObject.gameObject.name.Contains("Clock"))
                                {
                                    MovingObject.transform.position = new Vector3(hit.point.x, hit.point.y + clock.transform.position.y, hit.point.z);
                                    MovingObject.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));
                                }

                                // first remove
                                object_dict.Remove(MovingObject);
                                foreach(GameObject obj in object_dict.Keys){
                                    Constraint c = object_dict[obj];
                                    removeObj(MovingObject,c);
                                }
                                foreach(GameObject obj in plane_dict.Keys) {
                                    GameObject target = plane_dict[obj].Find(i => i == MovingObject);
                                    plane_dict[obj].Remove(target);
                                }
                                // then add
                                bool isVertical = hit.transform.gameObject.name.Contains("Wall");
                                addItem(MovingObject, isVertical, hit.transform.gameObject);
                            }
                        }
                        else{
                            MovingObject=null;
                            unhighlightAll();
                        }
                    }

                    if(EditModeOn ==2 && ClickCount == 2) //for remove
                    {
                        if (hit.transform.gameObject.tag == "Prefab" || hit.transform.gameObject.tag == "OPrefab")
                        {

                            MovingObject = hit.transform.gameObject;
                            EditModeOn = 2;
                            unhighlightAll();
                            highlightObj();
                        }
                        else{
                            MovingObject=null;
                            unhighlightAll();
                        }

                        if (MovingObject != null)
                        {
                            Destroy_B = true;
                            MovingObject.GetComponent<MeshRenderer>().material = Click_m;
                            int count_C = MovingObject.transform.childCount;
                            for (int i = 0; i < count_C; i++)
                            {
                                MovingObject.transform.GetChild(i).GetComponent<MeshRenderer>().material = Click_m;
                            }
                            Destory_Q.SetActive(true);
                        }
                    }

                    if (EditModeOn == 2 && ClickCount == 3) //for scale
                    {
                        unhighlightAll();
                        if (hit.transform.gameObject.name.Contains("Wall"))
                        {
                            MovingObject = hit.transform.gameObject;
                            highlightObj();
                            wall_adj = MovingObject;
                        }

                    }

                }

            }

        }
    }

        //mouse function
        void wClick()
        {
            whichobject = 1;
        }

        void fClick()
        {
            whichobject = 2;
        }

        void dClick()
        {
            whichobject = 3;
        }

        void cClick()
        {
            whichobject = 4;
        }

        void winClick()
        {
            whichobject = 5;
        }

        void fSlider_x(float value)
        {
        BaseFloor.transform.localScale = new Vector3(floor_slider_x.value, 1, floor_slider_y.value);

        }

        void fSlider_y(float value)
        {
        BaseFloor.transform.localScale = new Vector3(floor_slider_x.value, 1, floor_slider_y.value);
        }

        void wallslider(float value)
        {
        if (wall_adj != null)
        {
            wall_width = wall_slider.value;
            wall_adj.transform.localScale = new Vector3(4.5f * wall_width, 4, 0.1f);
        }
        }




    void EClick() //EditMode function
        {
            
            EditModeOn = 1;
            ClickCount++;
            GameObject[] prefabobj = GameObject.FindGameObjectsWithTag("Prefab");

        if (ClickCount != 0) {
            Show_Constraint_B.gameObject.SetActive(false);
            print("inactivate!!!");

        }

        if (ClickCount == 0)
            {
                EditeMode_T.text = "Edit Mode";
                Destroy_B = false;

   
            foreach (GameObject ob in prefabobj)
                {
                    ob.gameObject.GetComponent<MeshRenderer>().material = Unclick_m;
                } 
            }

            if(ClickCount ==1)
            {
                
                foreach (GameObject ob in prefabobj)
                {
                    ob.gameObject.GetComponent<MeshRenderer>().material = Click_m;
                }
                EditeMode_T.text = "Move";
                EditMode.GetComponent<Image>().color = new Color32(255, 120, 120,255);
            }

            if(ClickCount ==2)
            {
                EditeMode_T.text = "Remove";
                EditMode.GetComponent<Image>().color = new Color32(120, 255, 120,255);
            }
            if(ClickCount == 3)
            {
                EditeMode_T.text = "Scale";
                bool_wall_slider.SetActive(true);
                EditMode.GetComponent<Image>().color = new Color32(225, 255, 255, 255);
            }

            if(ClickCount==4)
            {
                wall_adj = null;
                bool_wall_slider.SetActive(false);
                wall_slider.value = 1.0f;
                
                ClickCount = 0;
                EditeMode_T.text = "Edit Mode";
                EditMode.GetComponent<Image>().color = new Color32(64,255,239,255);
            foreach (GameObject ob in prefabobj)
            {
                ob.GetComponent<MeshRenderer>().material = Unclick_m;
                ob.GetComponent<cakeslice.Outline>().enabled = false;
                int count_child = ob.transform.childCount;
                for (int i = 0; i < count_child; i++)
                {
                    ob.transform.GetChild(i).GetComponent<MeshRenderer>().material = Unclick_m;
                    ob.transform.GetChild(i).GetComponent<cakeslice.Outline>().enabled = false;
                }
            }

            GameObject[] oprefabobj = GameObject.FindGameObjectsWithTag("OPrefab");
            foreach (GameObject ob in oprefabobj)
            {
                ob.gameObject.GetComponent<MeshRenderer>().material = Unclick_go_m;
                ob.gameObject.GetComponent<cakeslice.Outline>().enabled = false;
                int count_child = ob.transform.childCount;
                for (int i = 0; i < count_child; i++)
                {
                    ob.transform.GetChild(i).GetComponent<MeshRenderer>().material = Unclick_go_m;
                    ob.transform.GetChild(i).GetComponent<cakeslice.Outline>().enabled = false;
                }
            }
        }
        //EditeMode_T.text[]
    }

    public void removeObj(GameObject obj, Constraint c){
        c.same_vertical_plane_with.Remove(obj);
        c.same_horizontal_plane_with.Remove(obj);
        c.align_x_with.Remove(obj);
        c.align_y_with.Remove(obj);
        c.align_z_with.Remove(obj);
    }

    public void ClickYes() //Question for Remove
    {
        // remove from object dict
        // remove wall or floor
        if(MovingObject.name.Contains("Wall") || MovingObject.name.Contains("Floor")){
            foreach(GameObject obj in plane_dict[MovingObject]){
                object_dict.Remove(obj);
                foreach(GameObject o in object_dict.Keys){
                    Constraint c = object_dict[o];
                    removeObj(obj,c);
                }
                Destroy(obj);
            }
            plane_dict.Remove(MovingObject);
        }
        // remove object
        else{
            object_dict.Remove(MovingObject);
            foreach(GameObject obj in object_dict.Keys){
                Constraint c = object_dict[obj];
                removeObj(MovingObject,c);
            }
            foreach(GameObject obj in plane_dict.Keys) {
                GameObject target = plane_dict[obj].Find(i => i == MovingObject);
                plane_dict[obj].Remove(target);
            }
        }

        Destroy_A = true;
        Destroy(MovingObject);
        Destroy_A = false;
        Destroy_B = false;
    }

    public void ClickNo()
    {
        Destroy_A = false;
        Destroy_B = false;
    }

    void gClick() {
        List<string> constraints = generate();
        string str = "";
        foreach (string s in constraints) {
            str += s;
            str += "\n";
        }
        Constraint_Text.text = str;
        Constraint_Panel.SetActive(true);
        Close_B.gameObject.SetActive(false);

        Close_B2.gameObject.SetActive(true);
        Export_B.gameObject.SetActive(true);

    }

    public void sClick() {
        HashSet<string> list = show_cons(MovingObject);
        string str = MovingObject.name.Split('_')[0];
        str += "\n";
        foreach (string s in list) {
            str += s;
            str += "\n";
        }
        Constraint_Text.text = str;
        Constraint_Panel.SetActive(true);
        Close_B2.gameObject.SetActive(false);
        Export_B.gameObject.SetActive(false);

        Close_B.gameObject.SetActive(true);
    }

    void closePanel() {
        Constraint_Panel.SetActive(false);

    }

    void exportClick() {
        var csv = new StringBuilder();
        string filePath = "Constraint.csv";

        foreach(string str in constraints) {
            string type = str.Split(new string[] { ": " }, StringSplitOptions.None)[0];
            string objlist = str.Split(new string[] { ": " }, StringSplitOptions.None)[1];
            string newLine = string.Format("{0},{1}", type, objlist);
            csv.AppendLine(newLine);  
        }

        //after your loop
        File.WriteAllText(filePath, csv.ToString());
        Constraint_Text.text = "Exported successful!";
        Invoke("closePanel", 2);
    }

    private void changeName(GameObject obj, GameObject prefab) {
        if (prefab == clock) {
            obj.name = "Clock_" + clockCount;
            clockCount++;
        }
        if (prefab == desk)  {
            obj.name = "Desk_" + deskCount;
            deskCount++;
        }
        if (prefab == wall) {
            obj.name = "Wall_" + wallCount;
            wallCount++;
        }
        if (prefab == floor)  {
            obj.name = "Floor_" + floorCount;
            floorCount++;
        }

        if (prefab == window)
        {
            obj.name = "Window_" + windowCount;
            windowCount++;
        }
        
    }

    private void addItem(GameObject obj, bool isVertical, GameObject key) {
        Constraint c = new Constraint();
        if(object_dict.ContainsKey(obj)){
            c = object_dict[obj];
        }
        if (isVertical) {
            c.attach_to_vertical_plane = true;
        } else {
            c.attach_to_horizontal_plane = true;
        }
        object_dict[obj] = c;
        List<GameObject> list = plane_dict[key];
        foreach(GameObject o in list){
            Constraint o_c = object_dict[o];
            if (isVertical) {
                c.same_vertical_plane_with.Add(o);
                o_c.same_vertical_plane_with.Add(obj);
            } else {
                c.same_horizontal_plane_with.Add(o);
                o_c.same_horizontal_plane_with.Add(obj);
            }
        }
        list.Add(obj);
        plane_dict[key] = list;
        isAlign(list, obj, key);
    }

    private HashSet<string> show_cons(GameObject obj){
        HashSet<string> cons = new HashSet<string>();
        Constraint l = object_dict[obj];
        if(l.attach_to_vertical_plane){
            cons.Add("Attached to vertical plane");
        }
        if(l.attach_to_horizontal_plane){
            cons.Add("Attached to horizontal plane");
        }
        if(l.same_vertical_plane_with.Count>=1){
            string c = "On the same vertical plane with ";
            c += string.Join(" ", l.same_vertical_plane_with.Select(x=>x.name.Split('_')[0]).ToArray());
            cons.Add(c);
        }
        if(l.same_horizontal_plane_with.Count>=1){
            string c = "On the same horizontal plane with ";
            c += string.Join(" ", l.same_horizontal_plane_with.Select(x=>x.name.Split('_')[0]).ToArray());
            cons.Add(c);
        }
        if(l.align_x_with.Count>=1){
            string c = "Aligned X with: ";
            c += string.Join(" ", l.align_x_with.Select(x=>x.name.Split('_')[0]).ToArray());
            cons.Add(c);
        }
        if(l.align_y_with.Count>=1){
            string c = "Aligned Y with: ";
            c += string.Join(" ", l.align_y_with.Select(x=>x.name.Split('_')[0]).ToArray());
            cons.Add(c);
        }
        if(l.align_z_with.Count>=1){
            string c = "Aligned Z with: ";
            c += string.Join(" ", l.align_z_with.Select(x=>x.name.Split('_')[0]).ToArray());
            cons.Add(c);
        }
        return cons;
    }

    private List<string> generate(){
        constraints = new List<string>();

        HashSet<string> vertical_plane = new HashSet<string>();
        HashSet<string> horizontal_plane = new HashSet<string>();

        

        foreach (GameObject key in plane_dict.Keys){
            List<GameObject> c_list = plane_dict[key];
            if(key.name.Contains("Wall")){
                foreach (GameObject o in c_list){
                    vertical_plane.Add(o.name.Split('_')[0]);
                }
                string same_plane = "On the same vertical plane: ";
                same_plane += "[" + string.Join("|", c_list.Select(x=>x.name.Split('_')[0]).ToArray()) + "]";
                if (c_list.Count >= 2) {
                    constraints.Add(same_plane);                    
                }
            }
            else if (key.name.Contains("Floor")){
                foreach (GameObject o in c_list){
                    horizontal_plane.Add(o.name.Split('_')[0]);
                }
                string same_plane = "On the same horizontal plane: ";
                same_plane += "[" + string.Join("|", c_list.Select(x=>x.name.Split('_')[0]).ToArray()) + "]";
                if (c_list.Count >= 2) {
                    constraints.Add(same_plane);                    
                }
            }
        }

        string cons = "";
        if(vertical_plane.Count>0){
            cons = "Attached to the vertical plane: " + "[" + string.Join("|", vertical_plane.ToArray()) + "]";
            constraints.Insert(0,cons);      
        }
        if(horizontal_plane.Count>0){
            cons =  "Attached to the horizontal plane: " + "[" + string.Join("|", horizontal_plane.ToArray()) + "]";
            constraints.Insert(1,cons);
        }
        
        HashSet<GameObject> seen_x = new HashSet<GameObject>();
        HashSet<GameObject> seen_y = new HashSet<GameObject>();
        HashSet<GameObject> seen_z = new HashSet<GameObject>();

        foreach(GameObject key in object_dict.Keys){
            Constraint c = object_dict[key];
            List<GameObject> list = c.align_x_with;
            if(list.Count>=1 && !seen_x.Contains(key)){
                cons = "Align X: [" + key.name.Split('_')[0] + "|" + string.Join("|", list.Select(x=>x.name.Split('_')[0]).ToArray()) + "]";
                foreach(GameObject obj in list){
                    seen_x.Add(obj);
                }
                constraints.Add(cons);
            }
            list = c.align_y_with;
            if(list.Count>=1 && !seen_y.Contains(key)){
                cons = "Align Y: [" + key.name.Split('_')[0] + "|" + string.Join("|", list.Select(x=>x.name.Split('_')[0]).ToArray()) + "]";
                foreach(GameObject obj in list){
                    seen_y.Add(obj);
                }
                constraints.Add(cons);
            }
            list = c.align_z_with;
            if(list.Count>=1 && !seen_z.Contains(key)){
                cons = "Align Z: [" + key.name.Split('_')[0] + "|" + string.Join("|", list.Select(x=>x.name.Split('_')[0]).ToArray()) + "]";
                foreach(GameObject obj in list){
                    seen_z.Add(obj);
                }
                constraints.Add(cons);
            }
            
        }

        print("Final Constraints: ");
        return constraints;
    }

    private void unhighlightAll() {
        GameObject[] prefabobj = GameObject.FindGameObjectsWithTag("Prefab");
        foreach (GameObject ob in prefabobj)
        {
            ob.GetComponent<MeshRenderer>().material = Unclick_m;
            ob.GetComponent<cakeslice.Outline>().enabled = false;
            int count_child = ob.transform.childCount;
            for (int i = 0; i < count_child; i++)
            {
                ob.transform.GetChild(i).GetComponent<MeshRenderer>().material = Unclick_m;
                ob.transform.GetChild(i).GetComponent<cakeslice.Outline>().enabled = false;
            }
        }

        GameObject[] oprefabobj = GameObject.FindGameObjectsWithTag("OPrefab");
        foreach (GameObject ob in oprefabobj)
        {
            ob.gameObject.GetComponent<MeshRenderer>().material = Unclick_go_m;
            ob.gameObject.GetComponent<cakeslice.Outline>().enabled = false;
            int count_child = ob.transform.childCount;
            for (int i = 0; i < count_child; i++)
            {
                ob.transform.GetChild(i).GetComponent<MeshRenderer>().material = Unclick_go_m;
                ob.transform.GetChild(i).GetComponent<cakeslice.Outline>().enabled = false;
            }
        }

    }


    private void highlightObj() {
        MovingObject.GetComponent<MeshRenderer>().material = Click_m;
        MovingObject.GetComponent<cakeslice.Outline>().enabled = true;
        int count = MovingObject.transform.childCount;
        for (int i = 0; i < count; i++)
        {
            MovingObject.transform.GetChild(i).GetComponent<MeshRenderer>().material = Click_m;
            MovingObject.transform.GetChild(i).GetComponent<cakeslice.Outline>().enabled = true;
        }
    }

    private void isAlign(List<GameObject> list, GameObject target, GameObject plane) {
        Vector3 pos = target.transform.position;
        float threshold = 0.1f;
        bool hasAlign = false;

        if (plane.name.Contains("Wall")) {
            print("Rotation: ");
            print(plane.transform.rotation);
            if (plane.transform.rotation.y == 0) {
                foreach(GameObject obj in list) {
                    if (obj != target) {
                        if (Mathf.Abs(obj.transform.position.x - pos.x) < threshold) {
                            hasAlign = true;
                            alignDict["align_y"].Add(obj);
                        }
                        if (Mathf.Abs(obj.transform.position.y - pos.y) < threshold) {
                            hasAlign = true;
                            alignDict["align_x"].Add(obj);
                        }
                    }
                }
            } else {
                foreach(GameObject obj in list) {
                    if (obj != target) {
                        if (Mathf.Abs(obj.transform.position.y - pos.y) < threshold) {
                            hasAlign = true;
                            alignDict["align_z"].Add(obj);
                        }
                        if (Mathf.Abs(obj.transform.position.z - pos.z) < threshold) {
                            hasAlign = true;
                            alignDict["align_y"].Add(obj);
                        }
                    }
                }
            }
        } else if (plane.name.Contains("Floor")) {
            foreach(GameObject obj in list) {
                if (obj != target) {
                    if (Mathf.Abs(obj.transform.position.x - pos.x) < threshold) {
                        hasAlign = true;
                        alignDict["align_z"].Add(obj);
                    }
                    if (Mathf.Abs(obj.transform.position.z - pos.z) < threshold) {
                        hasAlign = true;
                        alignDict["align_x"].Add(obj);
                    }
                }
            }
        }

        if (hasAlign) {
            alignedObj = target;
            string str = "Do you want to add the following constraints: \n\n";
            foreach (GameObject obj in alignDict["align_x"]) {
                string s = target.name.Split('_')[0] + " align x with " + obj.name.Split('_')[0] + "\n";
                str += s;
            }
            foreach (GameObject obj in alignDict["align_y"]) {
                string s = target.name.Split('_')[0] + " align y with " + obj.name.Split('_')[0] + "\n";
                str += s;
            }
            foreach (GameObject obj in alignDict["align_z"]) {
                string s = target.name.Split('_')[0] + " align z with " + obj.name.Split('_')[0] + "\n";
                str += s;
            }
            Align_Panel.SetActive(true);
            Align_Text.text = str;
        }
        

        print("plane rotation");
        print(plane.transform.rotation);
    }

    void alignYesClick() {
        Align_Panel.SetActive(false);
        // TODO: add constraint
        Constraint c = object_dict[alignedObj];
        foreach (GameObject obj in alignDict["align_x"]) {
            c.align_x_with.Add(obj);
            object_dict[obj].align_x_with.Add(alignedObj);
        }
        foreach (GameObject obj in alignDict["align_y"]) {
            c.align_y_with.Add(obj);
            object_dict[obj].align_y_with.Add(alignedObj);
        }
        foreach (GameObject obj in alignDict["align_z"]) {
            c.align_z_with.Add(obj);
            object_dict[obj].align_z_with.Add(alignedObj);
        }

        alignedObj = null;
        alignDict["align_x"] = new List<GameObject>();
        alignDict["align_y"] = new List<GameObject>();
        alignDict["align_z"] = new List<GameObject>();
    }

    void alignNoClick() {
        Align_Panel.SetActive(false);
        alignedObj = null;
        alignDict["align_x"] = new List<GameObject>();
        alignDict["align_y"] = new List<GameObject>();
        alignDict["align_z"] = new List<GameObject>();
    }
}
