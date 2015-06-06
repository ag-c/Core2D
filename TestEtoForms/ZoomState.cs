﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Test2d;

namespace TestEtoForms
{
    public class ZoomState
    {
        private EditorContext _context;
        private Action _invalidate;

        public float MinimumZoom = 0.01f;
        public float MaximumZoom = 1000.0f;
        public float ZoomSpeed = 3.5f;
        public float Zoom = 1f;
        public float PanX = 0f;
        public float PanY = 0f;
        public bool IsPanMode = false;
        public float PanOffsetX = 0f;
        public float PanOffsetY = 0f;
        public float OriginX = 0f;
        public float OriginY = 0f;
        public float StartX = 0f;
        public float StartY = 0f;
        public float WheelOriginX = 0f;
        public float WheelOriginY = 0f;
        public bool HaveWheelOrigin = false;
        public float WheelOffsetX = 0f;
        public float WheelOffsetY = 0f;

        public ZoomState(EditorContext context, Action invalidate)
        {
            _context = context;
            _invalidate = invalidate;
        }

        public void MiddleDown(float x, float y)
        {
            System.Diagnostics.Debug.Print("Pan Offset: {0}, {1}", PanOffsetX, PanOffsetY);

            StartX = x;
            StartY = y;

            OriginX = PanX;
            OriginY = PanY;

            IsPanMode = true;
        }

        public void PrimaryDown(float x, float y)
        {
            if (_context.Editor.IsLeftDownAvailable())
            {
                _context.Editor.LeftDown(
                    (x - PanX) / Zoom,
                    (y - PanY) / Zoom);
            }
        }

        public void AlternateDown(float x, float y)
        {
            if (_context.Editor.IsRightDownAvailable())
            {
                _context.Editor.RightDown(
                    (x - PanX) / Zoom,
                    (y - PanY) / Zoom);
            }
        }

        public void MiddleUp(float x, float y)
        {
            PanOffsetX += PanX - OriginX;
            PanOffsetY += PanY - OriginY;
            System.Diagnostics.Debug.Print("Pan Offset: {0}, {1}", PanOffsetX, PanOffsetY);
            IsPanMode = false;
        }

        public void PrimaryUp(float x, float y)
        {
            if (_context.Editor.IsLeftUpAvailable())
            {
                _context.Editor.LeftUp(
                    (x - PanX) / Zoom,
                    (y - PanY) / Zoom);
            }
        }

        public void AlternateUp(float x, float y)
        {
            if (_context.Editor.IsRightUpAvailable())
            {
                _context.Editor.RightUp(
                    (x - PanX) / Zoom,
                    (y - PanY) / Zoom);
            }
        }

        public void Move(float x, float y)
        {
            if (IsPanMode)
            {
                float vx = StartX - x;
                float vy = StartY - y;

                PanX = OriginX - vx;
                PanY = OriginY - vy;

                _context.Editor.Renderer.State.PanX = PanX;
                _context.Editor.Renderer.State.PanY = PanY;

                _invalidate();
            }
            else
            {
                if (_context.Editor.IsMoveAvailable())
                {
                    _context.Editor.Move(
                        (x - PanX) / Zoom,
                        (y - PanY) / Zoom);
                }
            }
        }

        public void Wheel(float x, float y, float delta)
        {
            float zoom = Zoom;
            zoom = delta > 0 ?
                zoom + zoom / ZoomSpeed :
                zoom - zoom / ZoomSpeed;

            if (zoom < MinimumZoom || zoom > MaximumZoom)
                return;

            if (!HaveWheelOrigin)
            {
                WheelOriginX = x;
                WheelOriginY = y;
                HaveWheelOrigin = true;
            }

            WheelOffsetX = x - WheelOriginX;
            WheelOffsetY = y - WheelOriginY;
            System.Diagnostics.Debug.Print("Wheel Offset: {0}, {1}", WheelOffsetX, WheelOffsetY);

            ZoomTo(
                zoom,
                x - PanOffsetX - WheelOffsetX,
                y - PanOffsetY - WheelOffsetY);

            _invalidate();
        }

        public void ZoomTo(float zoom, float rx, float ry)
        {
            float ax = (rx * Zoom) + PanX;
            float ay = (ry * Zoom) + PanY;

            Zoom = zoom;

            PanX = ax - (rx * Zoom);
            PanY = ay - (ry * Zoom);

            _context.Editor.Renderer.State.Zoom = Zoom;
            _context.Editor.Renderer.State.PanX = PanX;
            _context.Editor.Renderer.State.PanY = PanY;
        }

        public void ResetZoom()
        {
            Zoom = 1f;
            PanX = 0f;
            PanY = 0f;
            PanOffsetX = 0f;
            PanOffsetY = 0f;
            WheelOriginX = 0f;
            WheelOriginY = 0f;
            WheelOffsetX = 0f;
            WheelOffsetY = 0f;
            HaveWheelOrigin = false;

            _context.Editor.Renderer.State.Zoom = Zoom;
            _context.Editor.Renderer.State.PanX = PanX;
            _context.Editor.Renderer.State.PanY = PanY;
        }
    }
}
