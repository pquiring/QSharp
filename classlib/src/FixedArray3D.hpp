template<typename T>
Qt::QSharp::FixedArray1D<T> $check(Qt::QSharp::FixedArray1D<T> array) {
  return array;
}

template<typename T>
Qt::QSharp::FixedArray1D<T>* $check(Qt::QSharp::FixedArray1D<T>* array) {
  return array;
}

template<typename T>
Qt::QSharp::FixedArray1D<T> $check(Qt::QSharp::FixedArray1D<T> array, int offset, int length)
{if (offset + length > array->Length) $abe(); return array;}

template<typename T>
Qt::QSharp::FixedArray2D<T> $check(Qt::QSharp::FixedArray2D<T> array) {
  return array;
}

template<typename T>
Qt::QSharp::FixedArray2D<T>* $check(Qt::QSharp::FixedArray2D<T>* array) {
  return array;
}

template<typename T>
Qt::QSharp::FixedArray2D<T> $check(Qt::QSharp::FixedArray2D<T> array, int offset, int length)
{if (offset + length > array->Length) $abe(); return array;}

template<typename T>
Qt::QSharp::FixedArray3D<T> $check(Qt::QSharp::FixedArray3D<T> array) {
  return array;
}

template<typename T>
Qt::QSharp::FixedArray3D<T>* $check(Qt::QSharp::FixedArray3D<T>* array) {
  return array;
}

template<typename T>
Qt::QSharp::FixedArray3D<T> $check(Qt::QSharp::FixedArray3D<T> array, int offset, int length)
{if (offset + length > array->Length) $abe(); return array;}

