﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using DlibDotNet.Dnn;
using DlibDotNet.Extensions;
using DlibDotNet.Util;
using ElementTypes = DlibDotNet.StdVectorElementTypesRepository.ElementTypes;

// ReSharper disable once CheckNamespace
namespace DlibDotNet
{

    public sealed class StdVector<TItem> : DlibObject, IList<TItem>
    {

        #region Fields
        
        private readonly StdVectorImp<TItem> _Imp;

        #endregion

        #region Constructors
        
        public StdVector()
        {
            this.Param = null;
            this._Imp = CreateImp();
            this.NativePtr = this._Imp.Create();
        }

        public StdVector(IParameter param = null)
        {
            this.Param = param;
            this._Imp = CreateImp(param);
            this.NativePtr = this._Imp.Create();
        }

        public StdVector(int size, IParameter param = null)
        {
            this.Param = param;
            this._Imp = CreateImp(param);
            this.NativePtr = this._Imp.Create(size);
        }

        public StdVector(IEnumerable<TItem> data, IParameter param = null)
        {
            this.Param = param;
            this._Imp = CreateImp(param);
            this.NativePtr = this._Imp.Create(data);
        }

        internal StdVector(IntPtr ptr, IParameter param = null)
        {
            this.Param = param;
            this._Imp = CreateImp(param);
            this.NativePtr = ptr;
        }

        #endregion

        #region Properties

        public IntPtr ElementPtr
        {
            get
            {
                this.ThrowIfDisposed();
                return this._Imp.GetElementPtr(this.NativePtr);
            }
        }

        internal IParameter Param
        {
            get;
            private set;
        }

        public int Size
        {
            get
            {
                this.ThrowIfDisposed();
                return this._Imp.GetSize(this.NativePtr);
            }
        }

        #endregion

        #region Methods

        public TItem[] ToArray()
        {
            this.ThrowIfDisposed();
            return this._Imp.ToArray(this.NativePtr);
        }

        #region Overrides

        /// <summary>
        /// Releases all unmanaged resources.
        /// </summary>
        protected override void DisposeUnmanaged()
        {
            base.DisposeUnmanaged();

            if (this.NativePtr == IntPtr.Zero)
                return;

            this._Imp?.Dispose(this.NativePtr);
        }

        #endregion

        #region Helpers

        private static StdVectorImp<TItem> CreateImp(IParameter param = null)
        {
            if (StdVectorElementTypesRepository.SupportTypes.TryGetValue(typeof(TItem), out var type))
            {
                switch (type)
                {
                    case ElementTypes.Int32:
                        return new StdVectorInt32Imp() as StdVectorImp<TItem>;
                    case ElementTypes.UInt32:
                        return new StdVectorUInt32Imp() as StdVectorImp<TItem>;
                    case ElementTypes.Long:
                        return new StdVectorLongImp() as StdVectorImp<TItem>;
                    case ElementTypes.Double:
                        return new StdVectorDoubleImp() as StdVectorImp<TItem>;
                    case ElementTypes.VectorDouble:
                        return new StdVectorVectorDoubleImp() as StdVectorImp<TItem>;
                    case ElementTypes.Rectangle:
                        return new StdVectorRectangleImp() as StdVectorImp<TItem>;
                    case ElementTypes.Point:
                        return new StdVectorPointImp() as StdVectorImp<TItem>;
                    case ElementTypes.DPoint:
                        return new StdVectorDPointImp() as StdVectorImp<TItem>;
                    case ElementTypes.ChipDetails:
                        return new StdVectorChipDetailsImp() as StdVectorImp<TItem>;
                    case ElementTypes.StdString:
                        return new StdVectorStdStringImp() as StdVectorImp<TItem>;
                    case ElementTypes.FullObjectDetection:
                        return new StdVectorFullObjectDetectionImp() as StdVectorImp<TItem>;
                    case ElementTypes.RectDetection:
                        return new StdVectorRectDetectionImp() as StdVectorImp<TItem>;
                    case ElementTypes.ImageWindowOverlayLine:
                        return new StdVectorImageWindowOverlayLineImp() as StdVectorImp<TItem>;
                    case ElementTypes.PerspectiveWindowOverlayDot:
                        return new StdVectorPerspectiveWindowOverlayDotImp() as StdVectorImp<TItem>;
                    case ElementTypes.ImageDatasetMetadataImage:
                        return new StdVectorImageDatasetMetadataImageImp() as StdVectorImp<TItem>;
                    case ElementTypes.ImageDatasetMetadataBox:
                        return new StdVectorImageDatasetMetadataBoxImp() as StdVectorImp<TItem>;
                    case ElementTypes.MModRect:
                        return new StdVectorMModRectImp() as StdVectorImp<TItem>;
                    case ElementTypes.SurfPoint:
                        return new StdVectorSurfPointImp() as StdVectorImp<TItem>;
                    case ElementTypes.SamplePair:
                        return new StdVectorSamplePairImp() as StdVectorImp<TItem>;
                    case ElementTypes.StdVectorDouble:
                        return new StdVectorStdVectorDoubleImp() as StdVectorImp<TItem>;
                    case ElementTypes.StdVectorRectangle:
                        return new StdVectorStdVectorRectangleImp() as StdVectorImp<TItem>;
                    case ElementTypes.StdVectorMModRect:
                        return new StdVectorStdVectorMModRectImp() as StdVectorImp<TItem>;
                    case ElementTypes.StdVectorFullObjectDetection:
                        return new StdVectorStdVectorFullObjectDetectionImp() as StdVectorImp<TItem>;
                    case ElementTypes.DetectorWindowDetails:
                        return new StdVectorMModOptionsDetectorWindowDetailsImp() as StdVectorImp<TItem>;
                    case ElementTypes.OverlayRect:
                        return new StdVectorOverlayRectImp() as StdVectorImp<TItem>;
                }
            }
            else
            {
                var t = typeof(TItem);
                var matrix = typeof(MatrixBase);
                if (matrix.IsAssignableFrom(t))
                {
                    var arg = GenericHelper.GetTypeParameter(t);
                    if (!MatrixBase.TryParse(arg, out var r))
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);

                    var templateRows = 0;
                    var templateColumns = 0;

                    if (param != null)
                    {
                        if (!(param is MatrixTemplateSizeParameter sizeParameter))
                            throw new ArgumentOutOfRangeException(nameof(type), type, null);

                        templateRows = sizeParameter.TemplateRows;
                        templateColumns = sizeParameter.TemplateColumns;
                    }

                    switch (r)
                    {
                        case MatrixElementTypes.UInt8:
                            return new StdVectorMatrixImp<byte>(templateRows, templateColumns) as StdVectorImp<TItem>;
                        case MatrixElementTypes.UInt16:
                            return new StdVectorMatrixImp<ushort>(templateRows, templateColumns) as StdVectorImp<TItem>;
                        case MatrixElementTypes.UInt32:
                            return new StdVectorMatrixImp<uint>(templateRows, templateColumns) as StdVectorImp<TItem>;
                        case MatrixElementTypes.Int8:
                            return new StdVectorMatrixImp<sbyte>(templateRows, templateColumns) as StdVectorImp<TItem>;
                        case MatrixElementTypes.Int16:
                            return new StdVectorMatrixImp<short>(templateRows, templateColumns) as StdVectorImp<TItem>;
                        case MatrixElementTypes.Int32:
                            return new StdVectorMatrixImp<int>(templateRows, templateColumns) as StdVectorImp<TItem>;
                        case MatrixElementTypes.Float:
                            return new StdVectorMatrixImp<float>(templateRows, templateColumns) as StdVectorImp<TItem>;
                        case MatrixElementTypes.Double:
                            return new StdVectorMatrixImp<double>(templateRows, templateColumns) as StdVectorImp<TItem>;
                        case MatrixElementTypes.RgbPixel:
                            return new StdVectorMatrixImp<RgbPixel>(templateRows, templateColumns) as StdVectorImp<TItem>;
                        case MatrixElementTypes.RgbAlphaPixel:
                            return new StdVectorMatrixImp<RgbAlphaPixel>(templateRows, templateColumns) as StdVectorImp<TItem>;
                        case MatrixElementTypes.HsiPixel:
                            return new StdVectorMatrixImp<HsiPixel>(templateRows, templateColumns) as StdVectorImp<TItem>;
                    }
                }
            }

            throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        #endregion

        #endregion

        #region StdVectorImp

        private abstract class StdVectorImp<T>
        {

            #region Methods

            public abstract IntPtr Create();

            public abstract IntPtr Create(int size);

            public abstract IntPtr Create(IEnumerable<T> data);

            public abstract void Dispose(IntPtr ptr);

            public abstract IntPtr GetElementPtr(IntPtr ptr);

            public abstract int GetSize(IntPtr ptr);

            public abstract T[] ToArray(IntPtr ptr);

            #endregion

        }

        private sealed class StdVectorInt32Imp : StdVectorImp<int>
        {

            #region Methods

            public override IntPtr Create()
            {
                return NativeMethods.stdvector_int32_new1();
            }

            public override IntPtr Create(int size)
            {
                if (size < 0)
                    throw new ArgumentOutOfRangeException(nameof(size));

                return NativeMethods.stdvector_int32_new2(new IntPtr(size));
            }

            public override IntPtr Create(IEnumerable<int> data)
            {
                if (data == null)
                    throw new ArgumentNullException(nameof(data));

                var array = data.ToArray();
                return NativeMethods.stdvector_int32_new3(array, new IntPtr(array.Length));
            }

            public override void Dispose(IntPtr ptr)
            {
                NativeMethods.stdvector_int32_delete(ptr);
            }

            public override IntPtr GetElementPtr(IntPtr ptr)
            {
                return NativeMethods.stdvector_int32_getPointer(ptr);
            }

            public override int GetSize(IntPtr ptr)
            {
                return NativeMethods.stdvector_int32_getSize(ptr).ToInt32();
            }

            public override int[] ToArray(IntPtr ptr)
            {
                var size = this.GetSize(ptr);
                if (size == 0)
                    return new int[0];

                var dst = new int[size];
                var elementPtr = this.GetElementPtr(ptr);
                Marshal.Copy(elementPtr, dst, 0, dst.Length);
                return dst;
            }

            #endregion

        }

        private sealed class StdVectorUInt32Imp : StdVectorImp<uint>
        {

            #region Methods

            public override IntPtr Create()
            {
                return NativeMethods.stdvector_uint32_new1();
            }

            public override IntPtr Create(int size)
            {
                if (size < 0)
                    throw new ArgumentOutOfRangeException(nameof(size));

                return NativeMethods.stdvector_uint32_new2(new IntPtr(size));
            }

            public override IntPtr Create(IEnumerable<uint> data)
            {
                if (data == null)
                    throw new ArgumentNullException(nameof(data));

                var array = data.ToArray();
                return NativeMethods.stdvector_uint32_new3(array, new IntPtr(array.Length));
            }

            public override void Dispose(IntPtr ptr)
            {
                NativeMethods.stdvector_uint32_delete(ptr);
            }

            public override IntPtr GetElementPtr(IntPtr ptr)
            {
                return NativeMethods.stdvector_uint32_getPointer(ptr);
            }

            public override int GetSize(IntPtr ptr)
            {
                return NativeMethods.stdvector_uint32_getSize(ptr).ToInt32();
            }

            public override uint[] ToArray(IntPtr ptr)
            {
                var size = this.GetSize(ptr);
                if (size == 0)
                    return new uint[0];

                var dst = new uint[size];
                var elementPtr = this.GetElementPtr(ptr);
                Interop.InteropHelper.Copy(elementPtr, dst, (uint)dst.Length);
                return dst;
            }

            #endregion

        }

        private sealed class StdVectorLongImp : StdVectorImp<long>
        {

            #region Methods

            public override IntPtr Create()
            {
                return NativeMethods.stdvector_long_new1();
            }

            public override IntPtr Create(int size)
            {
                if (size < 0)
                    throw new ArgumentOutOfRangeException(nameof(size));

                return NativeMethods.stdvector_long_new2(new IntPtr(size));
            }

            public override IntPtr Create(IEnumerable<long> data)
            {
                if (data == null)
                    throw new ArgumentNullException(nameof(data));

                var array = data.ToArray();
                return NativeMethods.stdvector_long_new3(array, new IntPtr(array.Length));
            }

            public override void Dispose(IntPtr ptr)
            {
                NativeMethods.stdvector_long_delete(ptr);
            }

            public override IntPtr GetElementPtr(IntPtr ptr)
            {
                return NativeMethods.stdvector_long_getPointer(ptr);
            }

            public override int GetSize(IntPtr ptr)
            {
                return NativeMethods.stdvector_long_getSize(ptr).ToInt32();
            }

            public override long[] ToArray(IntPtr ptr)
            {
                var size = this.GetSize(ptr);
                if (size == 0)
                    return new long[0];

                var dst = new long[size];
                var elementPtr = this.GetElementPtr(ptr);
                Marshal.Copy(elementPtr, dst, 0, dst.Length);
                return dst;
            }

            #endregion

        }

        private sealed class StdVectorDoubleImp : StdVectorImp<double>
        {

            #region Methods

            public override IntPtr Create()
            {
                return NativeMethods.stdvector_double_new1();
            }

            public override IntPtr Create(int size)
            {
                if (size < 0)
                    throw new ArgumentOutOfRangeException(nameof(size));

                return NativeMethods.stdvector_double_new2(new IntPtr(size));
            }

            public override IntPtr Create(IEnumerable<double> data)
            {
                if (data == null)
                    throw new ArgumentNullException(nameof(data));

                var array = data.ToArray();
                return NativeMethods.stdvector_double_new3(array, new IntPtr(array.Length));
            }

            public override void Dispose(IntPtr ptr)
            {
                NativeMethods.stdvector_double_delete(ptr);
            }

            public override IntPtr GetElementPtr(IntPtr ptr)
            {
                return NativeMethods.stdvector_double_getPointer(ptr);
            }

            public override int GetSize(IntPtr ptr)
            {
                return NativeMethods.stdvector_double_getSize(ptr).ToInt32();
            }

            public override double[] ToArray(IntPtr ptr)
            {
                var size = this.GetSize(ptr);
                if (size == 0)
                    return new double[0];

                var dst = new double[size];
                var elementPtr = this.GetElementPtr(ptr);
                Marshal.Copy(elementPtr, dst, 0, dst.Length);
                return dst;
            }

            #endregion

        }

        private sealed class StdVectorChipDetailsImp : StdVectorImp<ChipDetails>
        {

            #region Methods

            public override IntPtr Create()
            {
                return NativeMethods.stdvector_chip_details_new1();
            }

            public override IntPtr Create(int size)
            {
                if (size < 0)
                    throw new ArgumentOutOfRangeException(nameof(size));

                return NativeMethods.stdvector_chip_details_new2(new IntPtr(size));
            }

            public override IntPtr Create(IEnumerable<ChipDetails> data)
            {
                if (data == null)
                    throw new ArgumentNullException(nameof(data));

                var array = data.Select(rectangle => rectangle.NativePtr).ToArray();
                return NativeMethods.stdvector_chip_details_new3(array, new IntPtr(array.Length));
            }

            public override void Dispose(IntPtr ptr)
            {
                NativeMethods.stdvector_chip_details_delete(ptr);
            }

            public override IntPtr GetElementPtr(IntPtr ptr)
            {
                return NativeMethods.stdvector_chip_details_getPointer(ptr);
            }

            public override int GetSize(IntPtr ptr)
            {
                return NativeMethods.stdvector_chip_details_getSize(ptr).ToInt32();
            }

            public override ChipDetails[] ToArray(IntPtr ptr)
            {
                var size = this.GetSize(ptr);
                if (size == 0)
                    return new ChipDetails[0];

                var dst = new IntPtr[size];
                NativeMethods.stdvector_chip_details_copy(ptr, dst);
                return dst.Select(p => new ChipDetails(p)).ToArray();
            }

            #endregion

        }

        private sealed class StdVectorStdStringImp : StdVectorImp<StdString>
        {

            #region Methods

            public override IntPtr Create()
            {
                return NativeMethods.stdvector_string_new1();
            }

            public override IntPtr Create(int size)
            {
                if (size < 0)
                    throw new ArgumentOutOfRangeException(nameof(size));

                return NativeMethods.stdvector_string_new2(new IntPtr(size));
            }

            public override IntPtr Create(IEnumerable<StdString> data)
            {
                if (data == null)
                    throw new ArgumentNullException(nameof(data));

                var array = data.Select(rectangle => rectangle.NativePtr).ToArray();
                return NativeMethods.stdvector_string_new3(array, new IntPtr(array.Length));
            }

            public override void Dispose(IntPtr ptr)
            {
                NativeMethods.stdvector_string_delete(ptr);
            }

            public override IntPtr GetElementPtr(IntPtr ptr)
            {
                return NativeMethods.stdvector_string_getPointer(ptr);
            }

            public override int GetSize(IntPtr ptr)
            {
                return NativeMethods.stdvector_string_getSize(ptr).ToInt32();
            }

            public override StdString[] ToArray(IntPtr ptr)
            {
                var size = this.GetSize(ptr);
                if (size == 0)
                    return new StdString[0];

                var dst = new IntPtr[size];
                NativeMethods.stdvector_string_copy(ptr, dst);
                return dst.Select(p => new StdString(p)).ToArray();
            }

            #endregion

        }

        private sealed class StdVectorFullObjectDetectionImp : StdVectorImp<FullObjectDetection>
        {

            #region Methods

            public override IntPtr Create()
            {
                return NativeMethods.stdvector_full_object_detection_new1();
            }

            public override IntPtr Create(int size)
            {
                if (size < 0)
                    throw new ArgumentOutOfRangeException(nameof(size));

                return NativeMethods.stdvector_full_object_detection_new2(new IntPtr(size));
            }

            public override IntPtr Create(IEnumerable<FullObjectDetection> data)
            {
                if (data == null)
                    throw new ArgumentNullException(nameof(data));

                var array = data.Select(rectangle => rectangle.NativePtr).ToArray();
                return NativeMethods.stdvector_full_object_detection_new3(array, new IntPtr(array.Length));
            }

            public override void Dispose(IntPtr ptr)
            {
                NativeMethods.stdvector_full_object_detection_delete(ptr);
            }

            public override IntPtr GetElementPtr(IntPtr ptr)
            {
                return NativeMethods.stdvector_full_object_detection_getPointer(ptr);
            }

            public override int GetSize(IntPtr ptr)
            {
                return NativeMethods.stdvector_full_object_detection_getSize(ptr).ToInt32();
            }

            public override FullObjectDetection[] ToArray(IntPtr ptr)
            {
                var size = this.GetSize(ptr);
                if (size == 0)
                    return new FullObjectDetection[0];

                var dst = new IntPtr[size];
                NativeMethods.stdvector_full_object_detection_copy(ptr, dst);
                return dst.Select(p => new FullObjectDetection(p)).ToArray();
            }

            #endregion

        }
        
        private sealed class StdVectorRectDetectionImp : StdVectorImp<RectDetection>
        {

            #region Methods

            public override IntPtr Create()
            {
                return NativeMethods.stdvector_rect_detection_new1();
            }

            public override IntPtr Create(int size)
            {
                if (size < 0)
                    throw new ArgumentOutOfRangeException(nameof(size));

                return NativeMethods.stdvector_rect_detection_new2(new IntPtr(size));
            }

            public override IntPtr Create(IEnumerable<RectDetection> data)
            {
                if (data == null)
                    throw new ArgumentNullException(nameof(data));

                var array = data.Select(rectangle => rectangle.NativePtr).ToArray();
                return NativeMethods.stdvector_rect_detection_new3(array, new IntPtr(array.Length));
            }

            public override void Dispose(IntPtr ptr)
            {
                NativeMethods.stdvector_rect_detection_delete(ptr);
            }

            public override IntPtr GetElementPtr(IntPtr ptr)
            {
                return NativeMethods.stdvector_rect_detection_getPointer(ptr);
            }

            public override int GetSize(IntPtr ptr)
            {
                return NativeMethods.stdvector_rect_detection_getSize(ptr).ToInt32();
            }

            public override RectDetection[] ToArray(IntPtr ptr)
            {
                var size = this.GetSize(ptr);
                if (size == 0)
                    return new RectDetection[0];

                var dst = new IntPtr[size];
                NativeMethods.stdvector_rect_detection_copy(ptr, dst);
                return dst.Select(p => new RectDetection(p)).ToArray();
            }

            #endregion

        }

        private sealed class StdVectorImageWindowOverlayLineImp : StdVectorImp<ImageWindow.OverlayLine>
        {

            #region Methods

            public override IntPtr Create()
            {
                return NativeMethods.stdvector_image_window_overlay_line_new1();
            }

            public override IntPtr Create(int size)
            {
                if (size < 0)
                    throw new ArgumentOutOfRangeException(nameof(size));

                return NativeMethods.stdvector_image_window_overlay_line_new2(new IntPtr(size));
            }

            public override IntPtr Create(IEnumerable<ImageWindow.OverlayLine> data)
            {
                if (data == null)
                    throw new ArgumentNullException(nameof(data));

                var array = data.Select(rectangle => rectangle.NativePtr).ToArray();
                return NativeMethods.stdvector_image_window_overlay_line_new3(array, new IntPtr(array.Length));
            }

            public override void Dispose(IntPtr ptr)
            {
                NativeMethods.stdvector_image_window_overlay_line_delete(ptr);
            }

            public override IntPtr GetElementPtr(IntPtr ptr)
            {
                return NativeMethods.stdvector_image_window_overlay_line_getPointer(ptr);
            }

            public override int GetSize(IntPtr ptr)
            {
                return NativeMethods.stdvector_image_window_overlay_line_getSize(ptr).ToInt32();
            }

            public override ImageWindow.OverlayLine[] ToArray(IntPtr ptr)
            {
                var size = this.GetSize(ptr);
                if (size == 0)
                    return new ImageWindow.OverlayLine[0];

                var dst = new IntPtr[size];
                NativeMethods.stdvector_image_window_overlay_line_copy(ptr, dst);
                return dst.Select(p => new ImageWindow.OverlayLine(p)).ToArray();
            }

            #endregion

        }

        private sealed class StdVectorImageDatasetMetadataImageImp : StdVectorImp<ImageDatasetMetadata.Image>
        {

            #region Methods

            public override IntPtr Create()
            {
                return NativeMethods.stdvector_image_dataset_metadata_image_new1();
            }

            public override IntPtr Create(int size)
            {
                if (size < 0)
                    throw new ArgumentOutOfRangeException(nameof(size));

                return NativeMethods.stdvector_image_dataset_metadata_image_new2(new IntPtr(size));
            }

            public override IntPtr Create(IEnumerable<ImageDatasetMetadata.Image> data)
            {
                if (data == null)
                    throw new ArgumentNullException(nameof(data));

                var array = data.Select(rectangle => rectangle.NativePtr).ToArray();
                return NativeMethods.stdvector_image_dataset_metadata_image_new3(array, new IntPtr(array.Length));
            }

            public override void Dispose(IntPtr ptr)
            {
                NativeMethods.stdvector_image_dataset_metadata_image_delete(ptr);
            }

            public override IntPtr GetElementPtr(IntPtr ptr)
            {
                return NativeMethods.stdvector_image_dataset_metadata_image_getPointer(ptr);
            }

            public override int GetSize(IntPtr ptr)
            {
                return NativeMethods.stdvector_image_dataset_metadata_image_getSize(ptr).ToInt32();
            }

            public override ImageDatasetMetadata.Image[] ToArray(IntPtr ptr)
            {
                var size = this.GetSize(ptr);
                if (size == 0)
                    return new ImageDatasetMetadata.Image[0];

                var dst = new IntPtr[size];
                NativeMethods.stdvector_image_dataset_metadata_image_copy(ptr, dst);
                return dst.Select(p => new ImageDatasetMetadata.Image(p)).ToArray();
            }

            #endregion

        }

        private sealed class StdVectorImageDatasetMetadataBoxImp : StdVectorImp<ImageDatasetMetadata.Box>
        {

            #region Methods

            public override IntPtr Create()
            {
                return NativeMethods.stdvector_image_dataset_metadata_box_new1();
            }

            public override IntPtr Create(int size)
            {
                if (size < 0)
                    throw new ArgumentOutOfRangeException(nameof(size));

                return NativeMethods.stdvector_image_dataset_metadata_box_new2(new IntPtr(size));
            }

            public override IntPtr Create(IEnumerable<ImageDatasetMetadata.Box> data)
            {
                if (data == null)
                    throw new ArgumentNullException(nameof(data));

                var array = data.Select(rectangle => rectangle.NativePtr).ToArray();
                return NativeMethods.stdvector_image_dataset_metadata_box_new3(array, new IntPtr(array.Length));
            }

            public override void Dispose(IntPtr ptr)
            {
                NativeMethods.stdvector_image_dataset_metadata_box_delete(ptr);
            }

            public override IntPtr GetElementPtr(IntPtr ptr)
            {
                return NativeMethods.stdvector_image_dataset_metadata_box_getPointer(ptr);
            }

            public override int GetSize(IntPtr ptr)
            {
                return NativeMethods.stdvector_image_dataset_metadata_box_getSize(ptr).ToInt32();
            }

            public override ImageDatasetMetadata.Box[] ToArray(IntPtr ptr)
            {
                var size = this.GetSize(ptr);
                if (size == 0)
                    return new ImageDatasetMetadata.Box[0];

                var dst = new IntPtr[size];
                NativeMethods.stdvector_image_dataset_metadata_box_copy(ptr, dst);
                return dst.Select(p => new ImageDatasetMetadata.Box(p)).ToArray();
            }

            #endregion

        }

        private sealed class StdVectorMModRectImp : StdVectorImp<MModRect>
        {

            #region Methods

            public override IntPtr Create()
            {
                return NativeMethods.stdvector_mmod_rect_new1();
            }

            public override IntPtr Create(int size)
            {
                if (size < 0)
                    throw new ArgumentOutOfRangeException(nameof(size));

                return NativeMethods.stdvector_mmod_rect_new2(new IntPtr(size));
            }

            public override IntPtr Create(IEnumerable<MModRect> data)
            {
                if (data == null)
                    throw new ArgumentNullException(nameof(data));

                var array = data.Select(rectangle => rectangle.NativePtr).ToArray();
                return NativeMethods.stdvector_mmod_rect_new3(array, new IntPtr(array.Length));
            }

            public override void Dispose(IntPtr ptr)
            {
                NativeMethods.stdvector_mmod_rect_delete(ptr);
            }

            public override IntPtr GetElementPtr(IntPtr ptr)
            {
                return NativeMethods.stdvector_mmod_rect_getPointer(ptr);
            }

            public override int GetSize(IntPtr ptr)
            {
                return NativeMethods.stdvector_mmod_rect_getSize(ptr).ToInt32();
            }

            public override MModRect[] ToArray(IntPtr ptr)
            {
                var size = this.GetSize(ptr);
                if (size == 0)
                    return new MModRect[0];

                var dst = new IntPtr[size];
                NativeMethods.stdvector_mmod_rect_copy(ptr, dst);
                return dst.Select(p => new MModRect(p)).ToArray();
            }

            #endregion

        }

        private sealed class StdVectorSurfPointImp : StdVectorImp<SurfPoint>
        {

            #region Methods

            public override IntPtr Create()
            {
                return NativeMethods.stdvector_surf_point_new1();
            }

            public override IntPtr Create(int size)
            {
                if (size < 0)
                    throw new ArgumentOutOfRangeException(nameof(size));

                return NativeMethods.stdvector_surf_point_new2(new IntPtr(size));
            }

            public override IntPtr Create(IEnumerable<SurfPoint> data)
            {
                if (data == null)
                    throw new ArgumentNullException(nameof(data));

                var array = data.Select(rectangle => rectangle.NativePtr).ToArray();
                return NativeMethods.stdvector_surf_point_new3(array, new IntPtr(array.Length));
            }

            public override void Dispose(IntPtr ptr)
            {
                NativeMethods.stdvector_surf_point_delete(ptr);
            }

            public override IntPtr GetElementPtr(IntPtr ptr)
            {
                return NativeMethods.stdvector_surf_point_getPointer(ptr);
            }

            public override int GetSize(IntPtr ptr)
            {
                return NativeMethods.stdvector_surf_point_getSize(ptr).ToInt32();
            }

            public override SurfPoint[] ToArray(IntPtr ptr)
            {
                var size = this.GetSize(ptr);
                if (size == 0)
                    return new SurfPoint[0];

                var dst = new IntPtr[size];
                NativeMethods.stdvector_surf_point_copy(ptr, dst);
                return dst.Select(p => new SurfPoint(p)).ToArray();
            }

            #endregion

        }

        private sealed class StdVectorSamplePairImp : StdVectorImp<SamplePair>
        {

            #region Methods

            public override IntPtr Create()
            {
                return NativeMethods.stdvector_sample_pair_new1();
            }

            public override IntPtr Create(int size)
            {
                if (size < 0)
                    throw new ArgumentOutOfRangeException(nameof(size));

                return NativeMethods.stdvector_sample_pair_new2(new IntPtr(size));
            }

            public override IntPtr Create(IEnumerable<SamplePair> data)
            {
                if (data == null)
                    throw new ArgumentNullException(nameof(data));

                var array = data.Select(rectangle => rectangle.NativePtr).ToArray();
                return NativeMethods.stdvector_sample_pair_new3(array, new IntPtr(array.Length));
            }

            public override void Dispose(IntPtr ptr)
            {
                NativeMethods.stdvector_sample_pair_delete(ptr);
            }

            public override IntPtr GetElementPtr(IntPtr ptr)
            {
                return NativeMethods.stdvector_sample_pair_getPointer(ptr);
            }

            public override int GetSize(IntPtr ptr)
            {
                return NativeMethods.stdvector_sample_pair_getSize(ptr).ToInt32();
            }

            public override SamplePair[] ToArray(IntPtr ptr)
            {
                var size = this.GetSize(ptr);
                if (size == 0)
                    return new SamplePair[0];

                var dst = new IntPtr[size];
                NativeMethods.stdvector_sample_pair_copy(ptr, dst);
                return dst.Select(p => new SamplePair(p)).ToArray();
            }

            #endregion

        }

        private sealed class StdVectorMatrixImp<TElement> : StdVectorImp<Matrix<TElement>>
            where TElement : struct
        {

            #region Fields

            private readonly MatrixElementTypes _Type;

            private readonly NativeMethods.MatrixElementType _NativeType;

            private readonly int _TemplateColumns;

            private readonly int _TemplateRows;

            #endregion

            #region Constructors

            public StdVectorMatrixImp(int templateRows, int templateColumns)
            {
                Matrix<TElement>.TryParse<TElement>(out var type);
                this._Type = type;
                this._NativeType = this._Type.ToNativeMatrixElementType();

                this._TemplateRows = templateRows;
                this._TemplateColumns = templateColumns;
            }

            #endregion

            #region Methods

            public override IntPtr Create()
            {
                return NativeMethods.stdvector_matrix_new1(this._NativeType, this._TemplateRows, this._TemplateColumns);
            }

            public override IntPtr Create(int size)
            {
                if (size < 0)
                    throw new ArgumentOutOfRangeException(nameof(size));

                return NativeMethods.stdvector_matrix_new2(this._NativeType, new IntPtr(size), this._TemplateRows, this._TemplateColumns);
            }

            public override IntPtr Create(IEnumerable<Matrix<TElement>> data)
            {
                if (data == null)
                    throw new ArgumentNullException(nameof(data));

                var array = data.Select(rectangle => rectangle.NativePtr).ToArray();
                return NativeMethods.stdvector_matrix_new3(this._NativeType, array, new IntPtr(array.Length), this._TemplateRows, this._TemplateColumns);
            }

            public override void Dispose(IntPtr ptr)
            {
                NativeMethods.stdvector_matrix_delete(this._NativeType, ptr, this._TemplateRows, this._TemplateColumns);
            }

            public override IntPtr GetElementPtr(IntPtr ptr)
            {
                return NativeMethods.stdvector_matrix_getPointer(this._NativeType, ptr, this._TemplateRows, this._TemplateColumns);
            }

            public override int GetSize(IntPtr ptr)
            {
                return NativeMethods.stdvector_matrix_getSize(this._NativeType, ptr, this._TemplateRows, this._TemplateColumns).ToInt32();
            }

            public override Matrix<TElement>[] ToArray(IntPtr ptr)
            {
                var size = this.GetSize(ptr);
                if (size == 0)
                    return new Matrix<TElement>[0];

                var dst = new IntPtr[size];
                NativeMethods.stdvector_matrix_copy(ptr, dst);
                return dst.Select(p => p != IntPtr.Zero ? new Matrix<TElement>(p, this._TemplateRows, this._TemplateColumns) : null).ToArray();
            }

            #endregion

        }

        private sealed class StdVectorPerspectiveWindowOverlayDotImp : StdVectorImp<PerspectiveWindow.OverlayDot>
        {

            #region Methods

            public override IntPtr Create()
            {
                return NativeMethods.stdvector_perspective_window_overlay_dot_new1();
            }

            public override IntPtr Create(int size)
            {
                if (size < 0)
                    throw new ArgumentOutOfRangeException(nameof(size));

                return NativeMethods.stdvector_perspective_window_overlay_dot_new2(new IntPtr(size));
            }

            public override IntPtr Create(IEnumerable<PerspectiveWindow.OverlayDot> data)
            {
                if (data == null)
                    throw new ArgumentNullException(nameof(data));

                var array = data.Select(rectangle => rectangle.NativePtr).ToArray();
                return NativeMethods.stdvector_perspective_window_overlay_dot_new3(array, new IntPtr(array.Length));
            }

            public override void Dispose(IntPtr ptr)
            {
                NativeMethods.stdvector_perspective_window_overlay_dot_delete(ptr);
            }

            public override IntPtr GetElementPtr(IntPtr ptr)
            {
                return NativeMethods.stdvector_perspective_window_overlay_dot_getPointer(ptr);
            }

            public override int GetSize(IntPtr ptr)
            {
                return NativeMethods.stdvector_perspective_window_overlay_dot_getSize(ptr).ToInt32();
            }

            public override PerspectiveWindow.OverlayDot[] ToArray(IntPtr ptr)
            {
                var size = this.GetSize(ptr);
                if (size == 0)
                    return new PerspectiveWindow.OverlayDot[0];

                var dst = new IntPtr[size];
                NativeMethods.stdvector_perspective_window_overlay_dot_copy(ptr, dst);
                return dst.Select(p => new PerspectiveWindow.OverlayDot(p)).ToArray();
            }

            #endregion

        }

        private sealed class StdVectorRectangleImp : StdVectorImp<Rectangle>
        {

            #region Methods

            public override IntPtr Create()
            {
                return NativeMethods.stdvector_rectangle_new1();
            }

            public override IntPtr Create(int size)
            {
                if (size < 0)
                    throw new ArgumentOutOfRangeException(nameof(size));

                return NativeMethods.stdvector_rectangle_new2(new IntPtr(size));
            }

            public override IntPtr Create(IEnumerable<Rectangle> data)
            {
                if (data == null)
                    throw new ArgumentNullException(nameof(data));

                var nativeArray = data.Select(rectangle => rectangle.ToNative()).ToArray();
                var array = nativeArray.Select(rectangle => rectangle.NativePtr).ToArray();

                // Rectangle is struct and it will be cloned in native domain.
                // So all cloned elemtent must be deleted!!
                return NativeMethods.stdvector_rectangle_new3(array, new IntPtr(array.Length));
            }

            public override void Dispose(IntPtr ptr)
            {
                // This function delete all element
                NativeMethods.stdvector_rectangle_delete(ptr);
            }

            public override IntPtr GetElementPtr(IntPtr ptr)
            {
                return NativeMethods.stdvector_rectangle_getPointer(ptr);
            }

            public override int GetSize(IntPtr ptr)
            {
                return NativeMethods.stdvector_rectangle_getSize(ptr).ToInt32();
            }

            public override Rectangle[] ToArray(IntPtr ptr)
            {
                var size = this.GetSize(ptr);
                if (size == 0)
                    return new Rectangle[0];

                var dst = new IntPtr[size];
                NativeMethods.stdvector_rectangle_copy(ptr, dst);

                // Rectangle class does not native pointer. In other words, 
                // native pointer should not be disposed on caller.
                // All elements of Point, DPoint and Rectangle are only disposed in Disposed methods.
                return dst.Select(p => new Rectangle(p, false)).ToArray();
            }

            #endregion

        }

        private sealed class StdVectorPointImp : StdVectorImp<Point>
        {

            #region Methods

            public override IntPtr Create()
            {
                return NativeMethods.stdvector_point_new1();
            }

            public override IntPtr Create(int size)
            {
                if (size < 0)
                    throw new ArgumentOutOfRangeException(nameof(size));

                return NativeMethods.stdvector_point_new2(new IntPtr(size));
            }

            public override IntPtr Create(IEnumerable<Point> data)
            {
                if (data == null)
                    throw new ArgumentNullException(nameof(data));

                var nativeArray = data.Select(point => point.ToNative()).ToArray();
                var array = nativeArray.Select(point => point.NativePtr).ToArray();

                // Point is struct and it will be cloned in native domain.
                // So all cloned elemtent must be deleted!!
                return NativeMethods.stdvector_point_new3(array, new IntPtr(array.Length));
            }

            public override void Dispose(IntPtr ptr)
            {
                // This function delete all element
                NativeMethods.stdvector_point_delete(ptr);
            }

            public override IntPtr GetElementPtr(IntPtr ptr)
            {
                return NativeMethods.stdvector_point_getPointer(ptr);
            }

            public override int GetSize(IntPtr ptr)
            {
                return NativeMethods.stdvector_point_getSize(ptr).ToInt32();
            }

            public override Point[] ToArray(IntPtr ptr)
            {
                var size = this.GetSize(ptr);
                if (size == 0)
                    return new Point[0];

                var dst = new IntPtr[size];
                NativeMethods.stdvector_point_copy(ptr, dst);

                // Point class does not native pointer. In other words, 
                // native pointer should not be disposed on caller.
                // All elements of Point, DPoint and Rectangle are only disposed in Disposed methods.
                return dst.Select(p => new Point(p, false)).ToArray();
            }

            #endregion

        }

        private sealed class StdVectorDPointImp : StdVectorImp<DPoint>
        {

            #region Methods

            public override IntPtr Create()
            {
                return NativeMethods.stdvector_dpoint_new1();
            }

            public override IntPtr Create(int size)
            {
                if (size < 0)
                    throw new ArgumentOutOfRangeException(nameof(size));

                return NativeMethods.stdvector_dpoint_new2(new IntPtr(size));
            }

            public override IntPtr Create(IEnumerable<DPoint> data)
            {
                if (data == null)
                    throw new ArgumentNullException(nameof(data));

                var nativeArray = data.Select(dpoint => dpoint.ToNative()).ToArray();
                var array = nativeArray.Select(dpoint => dpoint.NativePtr).ToArray();

                // DPoint is struct and it will be cloned in native domain.
                // So all cloned elemtent must be deleted!!
                return NativeMethods.stdvector_dpoint_new3(array, new IntPtr(array.Length));
            }

            public override void Dispose(IntPtr ptr)
            {
                // This function delete all element
                NativeMethods.stdvector_dpoint_delete(ptr);
            }

            public override IntPtr GetElementPtr(IntPtr ptr)
            {
                return NativeMethods.stdvector_dpoint_getPointer(ptr);
            }

            public override int GetSize(IntPtr ptr)
            {
                return NativeMethods.stdvector_dpoint_getSize(ptr).ToInt32();
            }

            public override DPoint[] ToArray(IntPtr ptr)
            {
                var size = this.GetSize(ptr);
                if (size == 0)
                    return new DPoint[0];

                var dst = new IntPtr[size];
                NativeMethods.stdvector_dpoint_copy(ptr, dst);

                // DPoint class does not native pointer. In other words, 
                // native pointer should not be disposed on caller.
                // All elements of Point, DPoint and Rectangle are only disposed in Disposed methods.
                return dst.Select(p => new DPoint(p, false)).ToArray();
            }

            #endregion

        }

        private sealed class StdVectorVectorDoubleImp : StdVectorImp<Vector<double>>
        {

            #region Methods

            public override IntPtr Create()
            {
                return NativeMethods.stdvector_vector_double_new1();
            }

            public override IntPtr Create(int size)
            {
                if (size < 0)
                    throw new ArgumentOutOfRangeException(nameof(size));

                return NativeMethods.stdvector_vector_double_new2(new IntPtr(size));
            }

            public override IntPtr Create(IEnumerable<Vector<double>> data)
            {
                if (data == null)
                    throw new ArgumentNullException(nameof(data));

                var array = data.Select(rectangle => rectangle.NativePtr).ToArray();
                return NativeMethods.stdvector_vector_double_new3(array, new IntPtr(array.Length));
            }

            public override void Dispose(IntPtr ptr)
            {
                NativeMethods.stdvector_vector_double_delete(ptr);
            }

            public override IntPtr GetElementPtr(IntPtr ptr)
            {
                return NativeMethods.stdvector_vector_double_getPointer(ptr);
            }

            public override int GetSize(IntPtr ptr)
            {
                return NativeMethods.stdvector_vector_double_getSize(ptr).ToInt32();
            }

            public override Vector<double>[] ToArray(IntPtr ptr)
            {
                var size = this.GetSize(ptr);
                if (size == 0)
                    return new Vector<double>[0];

                var dst = new IntPtr[size];
                NativeMethods.stdvector_vector_double_copy(ptr, dst);
                return dst.Select(p => new Vector<double>(p)).ToArray();
            }

            #endregion

        }

        private sealed class StdVectorStdVectorDoubleImp : StdVectorImp<StdVector<double>>
        {

            #region Methods

            public override IntPtr Create()
            {
                return NativeMethods.stdvector_stdvector_double_new1();
            }

            public override IntPtr Create(int size)
            {
                if (size < 0)
                    throw new ArgumentOutOfRangeException(nameof(size));

                return NativeMethods.stdvector_stdvector_double_new2(new IntPtr(size));
            }

            public override IntPtr Create(IEnumerable<StdVector<double>> data)
            {
                if (data == null)
                    throw new ArgumentNullException(nameof(data));

                var array = data.Select(vector => vector.NativePtr).ToArray();
                return NativeMethods.stdvector_stdvector_double_new3(array, new IntPtr(array.Length));
            }

            public override void Dispose(IntPtr ptr)
            {
                NativeMethods.stdvector_stdvector_double_delete(ptr);
            }

            public override IntPtr GetElementPtr(IntPtr ptr)
            {
                return NativeMethods.stdvector_stdvector_double_getPointer(ptr);
            }

            public override int GetSize(IntPtr ptr)
            {
                return NativeMethods.stdvector_stdvector_double_getSize(ptr).ToInt32();
            }

            public override StdVector<double>[] ToArray(IntPtr ptr)
            {
                var size = this.GetSize(ptr);
                if (size == 0)
                    return new StdVector<double>[0];

                var dst = new IntPtr[size];
                NativeMethods.stdvector_stdvector_double_copy(ptr, dst);
                return dst.Select(p => new StdVector<double>(p)).ToArray();
            }

            #endregion

        }

        private sealed class StdVectorStdVectorRectangleImp : StdVectorImp<StdVector<Rectangle>>
        {

            #region Methods

            public override IntPtr Create()
            {
                return NativeMethods.stdvector_stdvector_rectangle_new1();
            }

            public override IntPtr Create(int size)
            {
                if (size < 0)
                    throw new ArgumentOutOfRangeException(nameof(size));

                return NativeMethods.stdvector_stdvector_rectangle_new2(new IntPtr(size));
            }

            public override IntPtr Create(IEnumerable<StdVector<Rectangle>> data)
            {
                if (data == null)
                    throw new ArgumentNullException(nameof(data));

                var array = data.Select(vector => vector.NativePtr).ToArray();
                return NativeMethods.stdvector_stdvector_rectangle_new3(array, new IntPtr(array.Length));
            }

            public override void Dispose(IntPtr ptr)
            {
                NativeMethods.stdvector_stdvector_rectangle_delete(ptr);
            }

            public override IntPtr GetElementPtr(IntPtr ptr)
            {
                return NativeMethods.stdvector_stdvector_rectangle_getPointer(ptr);
            }

            public override int GetSize(IntPtr ptr)
            {
                return NativeMethods.stdvector_stdvector_rectangle_getSize(ptr).ToInt32();
            }

            public override StdVector<Rectangle>[] ToArray(IntPtr ptr)
            {
                var size = this.GetSize(ptr);
                if (size == 0)
                    return new StdVector<Rectangle>[0];

                var dst = new IntPtr[size];
                NativeMethods.stdvector_stdvector_rectangle_copy(ptr, dst);
                return dst.Select(p => new StdVector<Rectangle>(p)).ToArray();
            }

            #endregion

        }

        private sealed class StdVectorStdVectorMModRectImp : StdVectorImp<StdVector<MModRect>>
        {

            #region Methods

            public override IntPtr Create()
            {
                return NativeMethods.stdvector_stdvector_mmod_rect_new1();
            }

            public override IntPtr Create(int size)
            {
                if (size < 0)
                    throw new ArgumentOutOfRangeException(nameof(size));

                return NativeMethods.stdvector_stdvector_mmod_rect_new2(new IntPtr(size));
            }

            public override IntPtr Create(IEnumerable<StdVector<MModRect>> data)
            {
                if (data == null)
                    throw new ArgumentNullException(nameof(data));

                var array = data.Select(vector => vector.NativePtr).ToArray();
                return NativeMethods.stdvector_stdvector_mmod_rect_new3(array, new IntPtr(array.Length));
            }

            public override void Dispose(IntPtr ptr)
            {
                NativeMethods.stdvector_stdvector_mmod_rect_delete(ptr);
            }

            public override IntPtr GetElementPtr(IntPtr ptr)
            {
                return NativeMethods.stdvector_stdvector_mmod_rect_getPointer(ptr);
            }

            public override int GetSize(IntPtr ptr)
            {
                return NativeMethods.stdvector_stdvector_mmod_rect_getSize(ptr).ToInt32();
            }

            public override StdVector<MModRect>[] ToArray(IntPtr ptr)
            {
                var size = this.GetSize(ptr);
                if (size == 0)
                    return new StdVector<MModRect>[0];

                var dst = new IntPtr[size];
                NativeMethods.stdvector_stdvector_mmod_rect_copy(ptr, dst);
                return dst.Select(p => new StdVector<MModRect>(p)).ToArray();
            }

            #endregion

        }
        
        private sealed class StdVectorStdVectorFullObjectDetectionImp : StdVectorImp<StdVector<FullObjectDetection>>
        {

            #region Methods

            public override IntPtr Create()
            {
                return NativeMethods.stdvector_stdvector_full_object_detection_new1();
            }

            public override IntPtr Create(int size)
            {
                if (size < 0)
                    throw new ArgumentOutOfRangeException(nameof(size));

                return NativeMethods.stdvector_stdvector_full_object_detection_new2(new IntPtr(size));
            }

            public override IntPtr Create(IEnumerable<StdVector<FullObjectDetection>> data)
            {
                if (data == null)
                    throw new ArgumentNullException(nameof(data));

                var array = data.Select(vector => vector.NativePtr).ToArray();
                return NativeMethods.stdvector_stdvector_full_object_detection_new3(array, new IntPtr(array.Length));
            }

            public override void Dispose(IntPtr ptr)
            {
                NativeMethods.stdvector_stdvector_full_object_detection_delete(ptr);
            }

            public override IntPtr GetElementPtr(IntPtr ptr)
            {
                return NativeMethods.stdvector_stdvector_full_object_detection_getPointer(ptr);
            }

            public override int GetSize(IntPtr ptr)
            {
                return NativeMethods.stdvector_stdvector_full_object_detection_getSize(ptr).ToInt32();
            }

            public override StdVector<FullObjectDetection>[] ToArray(IntPtr ptr)
            {
                var size = this.GetSize(ptr);
                if (size == 0)
                    return new StdVector<FullObjectDetection>[0];

                var dst = new IntPtr[size];
                NativeMethods.stdvector_stdvector_full_object_detection_copy(ptr, dst);
                return dst.Select(p => new StdVector<FullObjectDetection>(p)).ToArray();
            }

            #endregion

        }

        private sealed class StdVectorMModOptionsDetectorWindowDetailsImp : StdVectorImp<MModOptions.DetectorWindowDetails>
        {

            #region Methods

            public override IntPtr Create()
            {
                return NativeMethods.stdvector_mmod_options_detector_window_details_new1();
            }

            public override IntPtr Create(int size)
            {
                if (size < 0)
                    throw new ArgumentOutOfRangeException(nameof(size));

                return NativeMethods.stdvector_mmod_options_detector_window_details_new2(new IntPtr(size));
            }

            public override IntPtr Create(IEnumerable<MModOptions.DetectorWindowDetails> data)
            {
                if (data == null)
                    throw new ArgumentNullException(nameof(data));

                var array = data.Select(vector => vector.NativePtr).ToArray();
                return NativeMethods.stdvector_mmod_options_detector_window_details_new3(array, new IntPtr(array.Length));
            }

            public override void Dispose(IntPtr ptr)
            {
                NativeMethods.stdvector_mmod_options_detector_window_details_delete(ptr);
            }

            public override IntPtr GetElementPtr(IntPtr ptr)
            {
                return NativeMethods.stdvector_mmod_options_detector_window_details_getPointer(ptr);
            }

            public override int GetSize(IntPtr ptr)
            {
                return NativeMethods.stdvector_mmod_options_detector_window_details_getSize(ptr).ToInt32();
            }

            public override MModOptions.DetectorWindowDetails[] ToArray(IntPtr ptr)
            {
                var size = this.GetSize(ptr);
                if (size == 0)
                    return new MModOptions.DetectorWindowDetails[0];

                var dst = new IntPtr[size];
                NativeMethods.stdvector_mmod_options_detector_window_details_copy(ptr, dst);
                return dst.Select(p => new MModOptions.DetectorWindowDetails(p)).ToArray();
            }

            #endregion

        }
        
        private sealed class StdVectorOverlayRectImp : StdVectorImp<ImageDisplay.OverlayRect>
        {

            #region Methods

            public override IntPtr Create()
            {
                return NativeMethods.stdvector_image_display_overlay_rect_new1();
            }

            public override IntPtr Create(int size)
            {
                if (size < 0)
                    throw new ArgumentOutOfRangeException(nameof(size));

                return NativeMethods.stdvector_image_display_overlay_rect_new2(new IntPtr(size));
            }

            public override IntPtr Create(IEnumerable<ImageDisplay.OverlayRect> data)
            {
                if (data == null)
                    throw new ArgumentNullException(nameof(data));

                var array = data.Select(rectangle => rectangle.NativePtr).ToArray();
                return NativeMethods.stdvector_image_display_overlay_rect_new3(array, new IntPtr(array.Length));
            }

            public override void Dispose(IntPtr ptr)
            {
                NativeMethods.stdvector_image_display_overlay_rect_delete(ptr);
            }

            public override IntPtr GetElementPtr(IntPtr ptr)
            {
                return NativeMethods.stdvector_image_display_overlay_rect_getPointer(ptr);
            }

            public override int GetSize(IntPtr ptr)
            {
                return NativeMethods.stdvector_image_display_overlay_rect_getSize(ptr).ToInt32();
            }

            public override ImageDisplay.OverlayRect[] ToArray(IntPtr ptr)
            {
                var size = this.GetSize(ptr);
                if (size == 0)
                    return new ImageDisplay.OverlayRect[0];

                var dst = new IntPtr[size];
                NativeMethods.stdvector_image_display_overlay_rect_copy(ptr, dst);
                return dst.Select(p => new ImageDisplay.OverlayRect(p)).ToArray();
            }

            #endregion

        }

        #endregion

        #region IEnumerable<TItem> Members

        public IEnumerator<TItem> GetEnumerator()
        {
            return ((IEnumerable<TItem>)this.ToArray()).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region IList<TItem> Members

        public void Add(TItem item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(TItem item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(TItem[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(TItem item)
        {
            throw new NotImplementedException();
        }

        public int Count => this.Size;

        public bool IsReadOnly
        {
            get;
        }

        public int IndexOf(TItem item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, TItem item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public TItem this[int index]
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        #endregion

    }

}