using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogicLayer.OrdersService.BusinessLogicLayer.DTO
{
    public record UserDTO(
            Guid UserId,
            string Email,
            string PersonName,
            string Gender
        );
}
