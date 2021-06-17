﻿using API.Data.Converter.Implementations;
using API.Data.VO.Rating;
using API.Exceptions;
using Database.Extension;
using Database.Model;
using Database.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Business.Implementations
{
    public class RatingBusinessImplementation : IRatingBusiness
    {
        private readonly User _requester;
        private readonly IRatingRepository _repository;
        private readonly IRepository<Appointment> _appointmentRepository;
        private readonly RatingConverter _converter;

        public RatingBusinessImplementation(User requester, IRatingRepository repository, IRepository<Appointment> appointmentRepository)
        {
            _requester = requester;
            _repository = repository;
            _appointmentRepository = appointmentRepository;
            _converter = new RatingConverter();
        }

        public async Task<RatingVO> CreateAsync(RatingCreateVO vo)
        {
            if (vo.Stars <= 0 || vo.Stars >= 6)
            {
                throw new APIException("Stars must be between 1 and 5");
            }
            var appointment = await _appointmentRepository.FindByIdAsync(vo.AppointmentId, nameof(Appointment.Service));
            if (appointment == null)
            {
                throw new NotFoundException($"Can't find appointment of id {vo.AppointmentId}");
            }
            if (await _repository.ExistsByAppointmentIdAsync(vo.AppointmentId))
            {
                throw new APIException("There is already one rating for this appointment");
            }
            if (!appointment.IsCompleted)
            {
                throw new APIException("Can't rate an appointment that is not yet completed");
            }
            if (appointment.ClientId != _requester.Id)
            {
                throw new UnauthorizedException("Only the appointment's client can rate it");
            }
            
            var entity = _converter.Parse(vo);
            entity = await _repository.AddAndUpdateConsultantRatingAsync(entity, appointment.Service.UserId);
            return _converter.Parse(entity);
        }

        public async Task<RatingVO> UpdateAsync(RatingEditVO vo)
        {
            if (vo.Stars <= 0 || vo.Stars >= 6)
            {
                throw new APIException("Stars must be between 1 and 5");
            }
            var rating = await _repository.FindByIdAsync(vo.Id);
            if (rating == null)
            {
                throw new NotFoundException($"Can't find rating of id {vo.Id}");
            }

            var appointment = await _appointmentRepository.FindByIdAsync(rating.AppointmentId, nameof(Appointment.Service));
            if (appointment == null)
            {
                throw new NotFoundException($"Can't find appointment of id {rating.AppointmentId}");
            }
            if (!appointment.IsCompleted)
            {
                throw new APIException("Can't rate an appointment that is not yet completed");
            }
            if (appointment.ClientId != _requester.Id)
            {
                throw new UnauthorizedException("Only the appointment's client can rate it");
            }
            var entity = _converter.Parse(vo);
            entity.AppointmentId = rating.AppointmentId;
            entity = await _repository.EditAndUpdateConsultantRatingAsync(entity, appointment.Service.UserId, rating.Stars);
            return _converter.Parse(entity);
        }
       
        public async Task<RatingVO> FindByIdAsync(long id)
        {
            var rating = await _repository.FindByIdAsync(id);
            if (rating == null)
            {
                throw new NotFoundException($"Rating id {id} not found");
            }

            return _converter.Parse(rating);
        }

        public async Task<List<RatingVO>> FindAllByConsultantIdAsync(long consultantId)
        {
            var all = await _repository.FindAllByConsultantIdAsync(consultantId,
                    $"{nameof(Rating.Appointment)}.{nameof(Appointment.Service)}"
                );
            return _converter.Parse(all);
        }

        public async Task DeleteAsync(long id)
        {
            var rating = await _repository.FindByIdAsync(id);
            if (rating == null)
            {
                throw new NotFoundException($"Can't find rating of id {id}");
            }
            var appointment = await _appointmentRepository.FindByIdAsync(rating.AppointmentId);
            if (appointment.ClientId != _requester.Id)
            {
                throw new UnauthorizedException("User is not allowed to delete this rating");
            }

            await _repository.DeleteAsync(id);
        }
    }
}