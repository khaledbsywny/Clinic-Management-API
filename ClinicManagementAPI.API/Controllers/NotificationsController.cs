using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using ClinicManagementAPI.Application.Common.DTOs;
using ClinicManagementAPI.Application.Common.Interfaces;
using ClinicManagementAPI.Domain.Entities;

namespace ClinicManagementAPI.API.Controllers;

[Authorize]
public class NotificationsController : BaseController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public NotificationsController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<NotificationDto>>> GetAll()
    {
        var notifications = await _unitOfWork.Repository<Notification>().GetAllAsync();
        var userNotifications = notifications.Where(n => n.UserId == GetCurrentUserId().ToString());
        var notificationDtos = _mapper.Map<IEnumerable<NotificationDto>>(userNotifications);
        return Ok(notificationDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<NotificationDto>> GetById(int id)
    {
        var notification = await _unitOfWork.Repository<Notification>().GetByIdAsync(id);
        if (notification == null)
            return NotFound();

        // Only allow users to view their own notifications
        if (notification.UserId != GetCurrentUserId().ToString())
            return Forbid();

        return HandleResult(_mapper.Map<NotificationDto>(notification));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<ActionResult<NotificationDto>> Create(CreateNotificationDto model)
    {
        var notification = _mapper.Map<Notification>(model);
        await _unitOfWork.Repository<Notification>().AddAsync(notification);
        await _unitOfWork.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = notification.Id }, _mapper.Map<NotificationDto>(notification));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateNotificationDto model)
    {
        var notification = await _unitOfWork.Repository<Notification>().GetByIdAsync(id);
        if (notification == null)
            return NotFound();

        // Only allow users to update their own notifications
        if (notification.UserId != GetCurrentUserId().ToString())
            return Forbid();

        _mapper.Map(model, notification);
        await _unitOfWork.Repository<Notification>().UpdateAsync(notification);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var notification = await _unitOfWork.Repository<Notification>().GetByIdAsync(id);
        if (notification == null)
            return NotFound();

        // Only allow users to delete their own notifications
        if (notification.UserId != GetCurrentUserId().ToString())
            return Forbid();

        await _unitOfWork.Repository<Notification>().DeleteAsync(notification);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("unread")]
    public async Task<ActionResult<IEnumerable<NotificationDto>>> GetUnread()
    {
        var notifications = await _unitOfWork.Repository<Notification>().GetAllAsync();
        var unreadNotifications = notifications
            .Where(n => n.UserId == GetCurrentUserId().ToString() && !n.IsRead);
        var notificationDtos = _mapper.Map<IEnumerable<NotificationDto>>(unreadNotifications);
        return Ok(notificationDtos);
    }

    [HttpPut("mark-all-read")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var notifications = await _unitOfWork.Repository<Notification>().GetAllAsync();
        var userNotifications = notifications.Where(n => n.UserId == GetCurrentUserId().ToString() && !n.IsRead);

        foreach (var notification in userNotifications)
        {
            notification.IsRead = true;
            await _unitOfWork.Repository<Notification>().UpdateAsync(notification);
        }

        await _unitOfWork.SaveChangesAsync();
        return NoContent();
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            throw new UnauthorizedAccessException("User is not authenticated");

        return int.Parse(userIdClaim.Value);
    }
} 