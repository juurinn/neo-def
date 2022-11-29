using UnityEngine;

/// <summary>
/// Handle player currency.
/// </summary>
/// <remarks>
/// <para>To use currency, use: <see cref="Currency.Use(int)"/></para>
/// </remarks>
public class Currency : MonoBehaviour {

    static public int amount { get; private set; }
    static private Currency instance;


    private void Awake() {
        // For making sure that there are always at the most one instance of Currency
        if (instance == null) {
            instance = this;
        } else {
            Debug.LogError("[Currency]: Trying to instance but another instance already exists! [" + gameObject + "]");
            Destroy(gameObject);
        }
    }


    private void Start() {
        // Get initial value for currency from config file
        amount = Config.BASE_CURRENCY;
    }


    /// <summary>
    /// Use currency, add or remove. Updates the currency on GUI.
    /// </summary>
    /// <remarks>
    /// <param name="m_Amount"> Amount of currency to be changed. Positive values for adding. | Negative values for removing. </param>
    /// </remarks>
    /// <returns> true: on success | false: on fail </returns>
    static public bool Use(int m_Amount) {
        // Try adding if amount is positive and vice versa
        float result = m_Amount < 0 
            ? Currency.Remove(m_Amount) 
            : Currency.Add(m_Amount);

        // If action was not succesfull
        if (result == -1) return false;

        // Update UI
        UIManager.instance.UpdateTextElement("currency", result.ToString());

        return true;
    }


    /// <summary>
    /// Is there enough currency to be able to spend m_Amount.
    /// </summary>
    /// <param name="m_Amount">Amount to be checked.</param>
    /// <returns> true: if can afford | false: if cannot afford</returns>
    static public bool CanAfford(int m_Amount) =>
        amount >= m_Amount;


    /// <summary>
    /// Add currency.
    /// </summary>
    /// <param name="m_Amount">Amount of currency to be added</param>
    /// <returns> Amount of currency: on success | -1: on fail </returns>
    static private int Add(int m_Amount) {
        if (m_Amount < 0) return -1;

        amount += m_Amount;
        return amount;
    }


    /// <summary>
    /// Remove currency.
    /// </summary>
    /// <param name="m_Amount">Amount of currency to be removed.</param>
    /// <returns> Amount of currency: on success | -1: on fail </returns>
    static private int Remove(int m_Amount) {
        if (m_Amount > 0 || amount + m_Amount < 0) return -1;

        amount += m_Amount;
        return amount;
    }
}
