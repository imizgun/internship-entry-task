using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicTacToeBank.Core.Domain;

namespace TicTacToeBank.DatabaseAccess.Configurations;

public class GameConfiguration : IEntityTypeConfiguration<Game> {
	public void Configure(EntityTypeBuilder<Game> builder) {
		builder.HasKey(x => x.Id);
		builder.Property(x => x.CreatedAt).IsRequired();
		builder.Property(x => x.Size).IsRequired();
		builder.Property(x => x.Status).IsRequired();
		builder.Property(x => x.OPlayerId).IsRequired();
		builder.Property(x => x.XPlayerId).IsRequired();
		builder.Property(x => x.MovesCount).IsRequired();

		builder.HasMany(x => x.Cells)
			.WithOne(x => x.Game)
			.HasForeignKey(x => x.GameId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}