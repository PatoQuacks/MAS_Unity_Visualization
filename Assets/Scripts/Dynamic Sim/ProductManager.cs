using UnityEngine;

public class ProductManager : MonoBehaviour
{
    [SerializeField] private GameObject productPrefab;

    private string[] productList;

    // Retrieval of data needed to generate AGVs
    void OnEnable()
    {
        NetworkReceiver.OnProductListReceived += HandleProductData;
    }

    void OnDisable()
    {
        NetworkReceiver.OnProductListReceived -= HandleProductData;
    }

    void HandleProductData(string[] _productList)
    {
        productList = _productList;
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        setProducts();
    }

    void setProducts()
    {
        foreach(string productName in productList)
        {
            // Creates the prefab and sets it as a child of the object running this script
            GameObject newProduct = Instantiate(productPrefab, transform);
            newProduct.transform.localPosition = new Vector3(0f,-2f,0f);
            newProduct.name = productName;
            /*
            // Set the # of the product in model
            newProduct.GetComponentInChildren<TextMeshProUGUI>().text = $"{productName[productName.Length - 1]}";
            */
            Debug.Log($"Setup for {productName}");
        }
    }
}
