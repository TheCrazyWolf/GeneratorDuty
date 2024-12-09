using GeneratorDuty.Models.Duty;
using GeneratorDuty.Repository.Database;
using Microsoft.EntityFrameworkCore;

namespace GeneratorDuty.Repository.Duty;

public class MembersRepository(DutyContext ef)
{
    public async Task<MemberDuty?> GetMemberDuty(long id)
    {
        return await ef.MemberDuties.FirstOrDefaultAsync(x=> x.Id == id);
    }

    public async Task<IEnumerable<MemberDuty>> GetMembersFromChat(long chatId)
    {
        return await ef.MemberDuties.Where(x=> x.IdPeer == chatId).ToListAsync();
    }

    public async Task<MemberDuty> Create(MemberDuty member)
    {
        await ef.AddAsync(member);
        await ef.SaveChangesAsync();
        return member;
    }

    public async Task<MemberDuty> Update(MemberDuty member)
    {
        ef.Update(member);
        await ef.SaveChangesAsync();
        return member;
    }

    public async Task Remove(MemberDuty memberDuty)
    {
        ef.Remove(memberDuty);
        await ef.SaveChangesAsync();
    }
}