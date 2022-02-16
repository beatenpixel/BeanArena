using MicroCrew.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Economy : Singleton<Economy> {

    public EconomyInventory playerInventory;

    public override void Init() {
        playerInventory = new EconomyInventory();
        playerInventory.InitCurrencies(CurrencyType.Coin | CurrencyType.Gem);

        var r = playerInventory.ExecuteTransaction(new Transaction_Add(new TransactionContent(
            Currency.Create(CurrencyType.Coin, Game.data.player.coins),
            Currency.Create(CurrencyType.Gem, Game.data.player.gems))), true);

        //var r = playerInventory.ExecuteTransaction(new Transaction_Add(new TransactionContent(Currency.Create(CurrencyType.Coin, 300))), true);
        MGameLoop.Update.Register(InternalUpdate);

        playerInventory.OnChangedInfo += (x) => {
            GE_OnCurrencyChanged.Invoke(new GE_OnCurrencyChanged(x));
        };

        GE_OnCurrencyChanged.Register((x) => {
            Debug.Log("changed");
            if (x.info.HasChanged(CurrencyType.Coin)) {
                (Currency a, Currency b) = x.info.Get(CurrencyType.Coin);
                Debug.Log(a.type + " changed from " + a.amount + " to " + b.amount);
            }
        });
    }

    protected override void Shutdown() {

    }

    private void InternalUpdate() {

        if (Input.GetKeyDown(KeyCode.Keypad7)) {
            var r = playerInventory.ExecuteTransaction(new Transaction_Add(
                new TransactionContent(Currency.Create(CurrencyType.Coin, 100))
            ), true);
        }

        if (Input.GetKeyDown(KeyCode.Keypad8)) {
            var r = playerInventory.ExecuteTransaction(new Transaction_Take(
                new TransactionContent(Currency.Create(CurrencyType.Coin, 37))
            ), true);
        }

    }

    public void AddCurrency(CurrencyType type, int amount) {
        playerInventory.ExecuteTransaction(new Transaction_Add(new TransactionContent(
                Currency.Create(type, amount))), true);
    }

    public bool HasCurrency(CurrencyType type, int amount) {
        return playerInventory.Has(new TransactionContent(new Currency(type, amount)));
    }

}

public struct OnCurrencyChangedInfo {
    public TransactionContent before;
    public TransactionContent after;

    public (Currency a, Currency b) Get(CurrencyType _type) {
        return (before.currencies.Find(x => x.type == _type), after.currencies.Find(x => x.type == _type));
    }

    public bool HasChanged(CurrencyType type) {
        Currency _before = before.currencies.Find(x => x.type == type);
        Currency _after = after.currencies.Find(x => x.type == type);

        if (_before.amount != _after.amount) {
            return true;
        } else {
            return false;
        }
    }
}

public class EconomyInventory {

    public Dictionary<CurrencyType, CurrencyStorage> all;

    public Action<OnCurrencyChangedInfo> OnChangedInfo;

    public EconomyInventory() {
        all = new Dictionary<CurrencyType, CurrencyStorage>();
    }

    public void InitCurrencies(CurrencyType type) {
        int t = (int)type;
        int n = 0;
        while (t > 0) {
            int c = t & 1;
            if (c != 0) {
                CurrencyType currency = (CurrencyType)(1 << n);
                all.Add(currency, new CurrencyStorage(new Currency(currency, 0)));
            }
            t >>= 1;
            n++;
        }
    }

    public TransactionResult ExecuteTransaction(Transaction transaction, bool apply) {
        Debug.Log("before: " + this.ToString());

        TransactionAttribute attrib = new TransactionAttribute();
        attrib.applyExecution = apply;

        TransactionContent contentBeforeExecute = new TransactionContent();
        foreach (var item in all.Values) {
            contentBeforeExecute.currencies.Add(item.currency);
        }

        TransactionResult result = default;

        if (transaction.type == TransactionType.Add) {
            Transaction_Add t = (Transaction_Add)transaction;
            result = Add(t.content, attrib);
        } else if (transaction.type == TransactionType.Take) {
            Transaction_Take t = (Transaction_Take)transaction;
            result = Take(t.content, attrib);
        }

        if (result.result == TransactionResultType.Success) {
            TransactionContent contentAfterExecute = new TransactionContent();
            foreach (var item in all.Values) {
                contentAfterExecute.currencies.Add(item.currency);
            }

            OnChangedInfo?.Invoke(new OnCurrencyChangedInfo() {
                before = contentBeforeExecute,
                after = contentAfterExecute
            });
        }

        Debug.Log("after: " + this.ToString());

        return result;
    }

    public TransactionResult Take(TransactionContent content, TransactionAttribute attr) {
        TransactionResult r = new TransactionResult();
        r.content = new TransactionContent();

        foreach (var c in content.currencies) {
            CurrencyStorage storage = all[c.type];
            Currency sc = storage.currency;

            if (sc.amount < c.amount) {
                r.content.currencies.Add(new Currency(c.type, c.amount - sc.amount));
            } else {
                if (attr.applyExecution) {
                    sc.amount -= c.amount;
                }
            }

            storage.currency = sc;
        }

        if (r.content.currencies.Count == 0) {
            r.result = TransactionResultType.Success;
        } else {
            r.result = TransactionResultType.Failed;
            r.resultInfo = TransactionResultInfo.NotEnoughCurrency;
        }

        return r;
    }

    public TransactionResult Add(TransactionContent content, TransactionAttribute attr) {
        TransactionResult r = new TransactionResult();
        r.content = new TransactionContent();

        foreach (var c in content.currencies) {
            CurrencyStorage storage = all[c.type];
            Currency sc = storage.currency;

            if (sc.amount + c.amount <= storage.capacity) {
                if (attr.applyExecution) {
                    sc.amount = sc.amount + c.amount;
                }
            } else {
                int overflow = (sc.amount + c.amount) - storage.capacity;

                r.content.currencies.Add(new Currency(c.type, overflow));
                if (attr.applyExecution) {
                    sc.amount = storage.capacity;
                }
            }

            storage.currency = sc;
        }

        if (r.content.currencies.Count == 0) {
            r.result = TransactionResultType.Success;
        } else {
            if (attr.failIfOverflow) {
                r.result = TransactionResultType.Failed;
                r.resultInfo = TransactionResultInfo.Overflow;
            } else {
                r.result = TransactionResultType.Success;
            }
        }

        return r;
    }

    public bool Has(TransactionContent content) {
        foreach (var currency in content.currencies) {
            if (!all.ContainsKey(currency.type)) {
                return false;
            }

            if (all[currency.type].currency.amount < currency.amount) {
                return false;
            }
        }

        return true;
    }

    public override string ToString() {
        string s = "";
        foreach (var item in all.Values) {
            s += item.ToString() + "\n";
        }

        return s;
    }

}

public class CurrencyStorage {
    public int capacity = int.MaxValue;
    public Currency currency;

    public CurrencyStorage(Currency _currency) {
        currency = _currency;
    }

    public override string ToString() {
        return "[storage] cap: " + capacity + " " + currency.ToString();
    }

}

public struct TransactionResult {
    public TransactionResultType result;
    public TransactionResultInfo resultInfo;
    public TransactionContent content;

    public override string ToString() {
        string s = result.ToString();
        s += "\n" + resultInfo.ToString();
        s += "\n" + content;
        return s;
    }
}

public enum TransactionResultInfo {
    None,
    Overflow,
    NotEnoughCurrency
}

public enum TransactionResultType {
    None,
    Success,
    Failed
}

public class TransactionAttribute {
    public bool applyExecution;
    public bool failIfOverflow;
}

public class Transaction {
    public TransactionType type;

    public Transaction(TransactionType _type) {
        type = _type;
    }
}

public class Transaction_Add : Transaction {

    public TransactionContent content;

    public Transaction_Add(TransactionContent _content) : base(TransactionType.Add) {
        content = _content;
    }

}

public class Transaction_Take : Transaction {

    public TransactionContent content;

    public Transaction_Take(TransactionContent _content) : base(TransactionType.Take) {
        content = _content;
    }

}

public enum TransactionType {
    None,
    Add,
    Take
}

public class TransactionContent {
    public List<Currency> currencies;

    public TransactionContent(params Currency[] _currencies) {
        currencies = new List<Currency>(_currencies);
    }

    public override string ToString() {
        string s = "";
        foreach (var item in currencies) {
            s += "[" + item.ToString() + "]";
        }

        return s;
    }
}

public struct Currency {
    public CurrencyType type;
    public int amount;

    public Currency(CurrencyType type, int amount) {
        this.type = type;
        this.amount = amount;
    }

    public static Currency Create(CurrencyType _type, int _amount) {
        return new Currency(_type, _amount);
    }

    public override string ToString() {
        return type + ":" + amount;
    }
}

public enum CurrencyType {
    None = 0,
    Coin = 1 << 0,
    Gem = 1 << 1,
    Iron = 1 << 2,
    Count = 3
}

public interface ITransactionUser {
    void SendOperationTo(ITransactionUser other, Transaction transaction);
    void ReceiveOperationFrom(ITransactionUser other, Transaction transaction);
}

public class GE_OnCurrencyChanged : MGameEvent<GE_OnCurrencyChanged> {
    public OnCurrencyChangedInfo info;

    public GE_OnCurrencyChanged(OnCurrencyChangedInfo _info) {
        info = _info;
    }
}