using AutoMapper;
using TicTacToeBank.Application.DTOs;
using TicTacToeBank.Core.Domain;

namespace TicTacToeBank;

public class AutoMapperProfile : Profile {
	public AutoMapperProfile() {
		CreateMap<Game, GameDto>()
			.PreserveReferences()
			.MaxDepth(3);
		
		CreateMap<GameCell, CellDto>()
			.PreserveReferences()
			.MaxDepth(3);
	}
}