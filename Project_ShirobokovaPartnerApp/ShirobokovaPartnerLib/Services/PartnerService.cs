using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShirobokovaPartnerLib.Data;
using ShirobokovaPartnerLib.Models;

namespace ShirobokovaPartnerLib.Services
{
    public class PartnerService
    {
        private readonly AppDbContext _context;

        public PartnerService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Partner>> GetAllPartnersAsync()
        {
            var partners = await _context.Partners
                .Include(p => p.PartnerType)
                .ToListAsync();

            foreach (var partner in partners)
            {
                partner.TotalSales = await _context.Sales
                    .Where(s => s.PartnerId == partner.Id)
                    .SumAsync(s => s.TotalAmount);
            }

            return partners;
        }

        public async Task<Partner?> GetPartnerByIdAsync(int id)
        {
            var partner = await _context.Partners
                .Include(p => p.PartnerType)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (partner != null)
            {
                partner.TotalSales = await _context.Sales
                    .Where(s => s.PartnerId == partner.Id)
                    .SumAsync(s => s.TotalAmount);
            }

            return partner;
        }

        public async Task<bool> AddPartnerAsync(Partner partner)
        {
            try
            {
                partner.CreatedAt = DateTime.Now;
                partner.UpdatedAt = DateTime.Now;

                await _context.Partners.AddAsync(partner);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdatePartnerAsync(Partner partner)
        {
            try
            {
                var existing = await _context.Partners.FindAsync(partner.Id);
                if (existing == null) return false;

                existing.PartnerTypeId = partner.PartnerTypeId;
                existing.Name = partner.Name;
                existing.Rating = partner.Rating;
                existing.Address = partner.Address;
                existing.DirectorName = partner.DirectorName;
                existing.Phone = partner.Phone;
                existing.Email = partner.Email;
                existing.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeletePartnerAsync(int id)
        {
            try
            {
                var partner = await _context.Partners.FindAsync(id);
                if (partner == null) return false;

                _context.Partners.Remove(partner);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<Sale>> GetSalesHistoryAsync(int partnerId)
        {
            return await _context.Sales
                .Where(s => s.PartnerId == partnerId)
                .OrderByDescending(s => s.SaleDate)
                .ToListAsync();
        }

        public async Task<List<PartnerType>> GetAllPartnerTypesAsync()
        {
            return await _context.PartnerTypes.ToListAsync();
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }
    }
}