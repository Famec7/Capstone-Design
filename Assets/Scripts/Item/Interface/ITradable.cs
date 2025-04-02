using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface ITradable
{
    //거래소에 거래될 때 호출되는 함수
    void TradeToExchange();
}
