using GeneratorDuty.Models.Duty;

namespace GeneratorDuty.Services;

public class MemoryExceptionDuty
{
    private readonly List<MemberDutyException> _memberDuties = new List<MemberDutyException>();

    public void AddMemberDuty(MemberDuty memberDuty)
    {
        var upcast = new MemberDutyException()
        {
            Id = memberDuty.Id,
            IdPeer = memberDuty.IdPeer,
            MemberNameDuty = memberDuty.MemberNameDuty,
            DateTimeAdded = DateTime.Now
        };
        _memberDuties.Add(upcast);
        RefreshMemberDuties();
    }
    
    public void AddMemberDuty(MemberDutyException memberDuty)
    {
        _memberDuties.Add(memberDuty);
        RefreshMemberDuties();
    }

    public List<MemberDutyException> GetFromChats(long chatId)
    {
        RefreshMemberDuties();
        return _memberDuties.Where(x=> x.IdPeer == chatId).ToList();
    }

    private void RefreshMemberDuties()
    {
        var toBeDeleted = _memberDuties
            .Where(x=> (DateTime.Now - x.DateTimeAdded).TotalDays >= 1);

        foreach (var item in toBeDeleted.ToList())
            _memberDuties.Remove(item);
    }
}

public class MemberDutyException : MemberDuty
{
    public DateTime DateTimeAdded { get; set; }
}