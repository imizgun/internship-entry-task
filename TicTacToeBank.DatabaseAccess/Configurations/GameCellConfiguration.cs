using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicTacToeBank.Core.Domain;

namespace TicTacToeBank.DatabaseAccess.Configurations;

public class GameCellConfiguration : IEntityTypeConfiguration<GameCell> {
	public void Configure(EntityTypeBuilder<GameCell> builder) {
		builder.HasKey(x => x.Id);
		builder.Property(x => x.Row).IsRequired();
		builder.Property(x => x.Column).IsRequired();
		builder.Property(x => x.GameId).IsRequired();
		builder.Property(x => x.Status).IsRequired();
	}
}