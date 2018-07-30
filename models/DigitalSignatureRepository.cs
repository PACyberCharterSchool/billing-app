using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using System;
using System.Linq;
using System.Collections.Generic;

using static models.Common.PropertyMerger;

namespace models
{
  public interface IDigitalSignatureRepository
  {
    DigitalSignature Create(DigitalSignature create);
    DigitalSignature Create(DateTime time, DigitalSignature create);
    bool Delete(int id);
    DigitalSignature Get(int id);
    IEnumerable<DigitalSignature> GetMany();
    DigitalSignature Update(DateTime time, DigitalSignature update);
    DigitalSignature Update(DigitalSignature update);
  }

  public class DigitalSignatureRepository : IDigitalSignatureRepository
  {
    private readonly DbSet<DigitalSignature> _signatures;
    private readonly ILogger<DigitalSignatureRepository> _logger;

    public DigitalSignatureRepository(PacBillContext context, ILogger<DigitalSignatureRepository> logger)
    {
      _signatures = context.DigitalSignatures;
      _logger = logger;
    }

    public IList<DigitalSignature> CreateMany(DateTime time, IList<DigitalSignature> creates)
    {
      foreach (var create in creates) {
        create.Created = time;
        create.LastUpdated = time;
      }

      _signatures.AddRange(creates);
      return creates;
    }

    public DigitalSignature Create(DateTime time, DigitalSignature create) => CreateMany(time, new[] { create })[0];

    public DigitalSignature Create(DigitalSignature create) => Create(DateTime.Now, create);

    public bool Delete(int id)
    {
      var signature = _signatures.SingleOrDefault(ds => ds.Id == id);
      if (signature != null) {
        _signatures.Remove(signature);
        return true;
      }

      return false;
    }

    public DigitalSignature Get(int id) => _signatures.SingleOrDefault(ds => ds.Id == id);

    public IEnumerable<DigitalSignature> GetMany() => _signatures.OrderBy(sd => sd.Title);

		private static IList<string> _excludedFields = new List<string>{
			nameof(Refund.Id),
			nameof(Refund.Created),
			nameof(Refund.LastUpdated),
		};

    public IList<DigitalSignature> UpdateMany(DateTime time, IList<DigitalSignature> updates)
    {
      var updated = new List<DigitalSignature>();
      foreach (var update in updates) {
        var signature = _signatures.SingleOrDefault(sd => sd.Id == update.Id);
        if (signature == null)
          throw new NotFoundException(typeof(DigitalSignature), update.Id);

        MergeProperties(signature, update, _excludedFields);
        signature.LastUpdated = time;

        _signatures.Update(signature);
        updated.Add(signature);
      }

      return updated;
    }

    public DigitalSignature Update(DateTime time, DigitalSignature update) => UpdateMany(time, new[] { update })[0]; 
    public DigitalSignature Update(DigitalSignature update) => Update(DateTime.Now, update);
  }
}
