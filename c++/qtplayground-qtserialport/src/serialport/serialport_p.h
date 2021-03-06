/****************************************************************************
**
** Copyright (C) 2011-2012 Denis Shienkov <scapig2@yandex.ru>
** Copyright (C) 2011 Sergey Belyashov <Sergey.Belyashov@gmail.com>
** Copyright (C) 2012 Laszlo Papp <lpapp@kde.org>
** Contact: http://www.qt-project.org/
**
** This file is part of the QtSerialPort module of the Qt Toolkit.
**
** $QT_BEGIN_LICENSE:LGPL$
** GNU Lesser General Public License Usage
** This file may be used under the terms of the GNU Lesser General Public
** License version 2.1 as published by the Free Software Foundation and
** appearing in the file LICENSE.LGPL included in the packaging of this
** file. Please review the following information to ensure the GNU Lesser
** General Public License version 2.1 requirements will be met:
** http://www.gnu.org/licenses/old-licenses/lgpl-2.1.html.
**
** In addition, as a special exception, Nokia gives you certain additional
** rights. These rights are described in the Nokia Qt LGPL Exception
** version 1.1, included in the file LGPL_EXCEPTION.txt in this package.
**
** GNU General Public License Usage
** Alternatively, this file may be used under the terms of the GNU General
** Public License version 3.0 as published by the Free Software Foundation
** and appearing in the file LICENSE.GPL included in the packaging of this
** file. Please review the following information to ensure the GNU General
** Public License version 3.0 requirements will be met:
** http://www.gnu.org/copyleft/gpl.html.
**
** Other Usage
** Alternatively, this file may be used in accordance with the terms and
** conditions contained in a signed written agreement between you and Nokia.
**
**
**
**
**
**
** $QT_END_LICENSE$
**
****************************************************************************/

#ifndef SERIALPORT_P_H
#define SERIALPORT_P_H

#include "serialport.h"

#if (QT_VERSION >= QT_VERSION_CHECK(5, 0, 0))
#include <private/qringbuffer_p.h>
#else
#include "qt4support/qringbuffer_p.h"
#endif

QT_BEGIN_NAMESPACE_SERIALPORT

class SerialPortPrivateData
{
    Q_DECLARE_PUBLIC(SerialPort)
public:
    enum IoConstants {
        ReadChunkSize = 512,
        WriteChunkSize = 512
    };

    SerialPortPrivateData(SerialPort *q);
    int timeoutValue(int msecs, int elapsed);

    qint64 readBufferMaxSize;
    QRingBuffer readBuffer;
    QRingBuffer writeBuffer;
    SerialPort::PortError portError;
    QString systemLocation;
    qint32 inputRate;
    qint32 outputRate;
    SerialPort::DataBits dataBits;
    SerialPort::Parity parity;
    SerialPort::StopBits stopBits;
    SerialPort::FlowControl flow;
    SerialPort::DataErrorPolicy policy;
    bool restoreSettingsOnClose;
    SerialPort * const q_ptr;
};

QT_END_NAMESPACE_SERIALPORT

#endif // SERIALPORT_P_H
