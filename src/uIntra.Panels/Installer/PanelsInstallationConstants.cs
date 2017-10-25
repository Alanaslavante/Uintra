﻿namespace uIntra.Panels.Installer
{
    public class PanelsInstallationConstants
    {
        public class DocumentTypeNames
        {
            public const string Panel = "Panel";
            public const string GlobalPanelFolder = "Global Panel Folder";
        }
        public class DocumentTypeAliases
        {
            public const string Panel = "panel";
            public const string GlobalPanelFolder = "globalPanelFolder";
        }
        public class DocumentTypePropertyNames
        {
            public const string PanelConfig = "Panel Config";
        }
        public class DocumentTypePropertyAliases
        {
            public const string PanelConfig = uIntra.Core.Constants.GridEditorConstants.PanelConfigPropertyAlias;
        }

        public class DataTypeNames
        {
            public const string PanelPicker = "Panel picker";
        }

        public class DataTypePreValueAliases
        {
            public const string PanelPickerConfig = "config";
        }

        public class DataTypePropertyEditors
        {
            public const string PanelPicker = "custom.PanelPicker";
        }
        public class DataTypePropertyAliases
        {
            public const string Assembly = "assembly";
            public const string Enum = "enum";
        }

        public class DocumentTypeIcons
        {
            public const string Panel = "icon-desktop";

        }
    }
}
