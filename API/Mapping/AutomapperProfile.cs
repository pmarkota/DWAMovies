using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace API.Mapping
{
    public class AutomapperProfile :Profile
    {
        public AutomapperProfile()
        {
            CreateMap<DAL.DALModels.Video, DAL.BLModels.BLVideo>().ReverseMap();
            CreateMap<DAL.DALModels.Genre, DAL.BLModels.BLGenre>().ReverseMap();
            CreateMap<DAL.DALModels.Tag, DAL.BLModels.BLTag>().ReverseMap();
            CreateMap<DAL.DALModels.User, DAL.BLModels.BLUser>().ReverseMap();
            CreateMap<DAL.DALModels.Notification, DAL.BLModels.BLNotification>().ReverseMap();
            CreateMap<DAL.DALModels.Image, DAL.BLModels.BLImage>().ReverseMap();
        }
    }
}
