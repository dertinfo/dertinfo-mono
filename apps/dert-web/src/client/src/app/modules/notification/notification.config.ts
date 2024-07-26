export class Constants {

    public static readonly controllerUrl: string = 'notification';

    public static readonly checkUrl: string = `${Constants.controllerUrl}/check`;

    public static readonly messagesUrl: string = `${Constants.controllerUrl}/list`;

    public static readonly summariesUrl: string = `${Constants.controllerUrl}`;

    public static readonly createUrl: string = `${Constants.controllerUrl}`;

    public static readonly deleteUrl: string = `${Constants.controllerUrl}`;

    public static readonly detailsUrl: string = 'detail';

    public static readonly dismissUrl: string = 'dismiss';

    public static readonly acknowledgeUrl: string = 'acknowledge';

}
