// 외부 이벤트에서 스탯을 조작할 때 사용할 클래스
// 나중에 추가로 Source(스탯 조작 출처)도 담을 수 있음 ( GAS같이 )
[System.Serializable]
public class StatModifier
{
    public StatType Type;  
    public float Amount;     

    // 스탯 조정 함수
    public void Apply(Stat stat)
    {
        if (stat.type == Type)
        {
            stat.Modify(Amount);
        }
    }
}
